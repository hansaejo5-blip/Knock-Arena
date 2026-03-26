using System;
using UnityEngine;

namespace Breakline.Runtime.Tiles
{
    // Suggested prefab:
    // - Root: DestructibleTile + one or more Collider2D components
    // - Child visuals: IntactVisual, CrackedVisual, BrokenVisual
    // - Broken tiles disable support colliders so players fall through
    public sealed class DestructibleTile : MonoBehaviour
    {
        [Header("Durability")]
        [SerializeField] private int maxDurability = 2;
        [SerializeField] private float damageLockoutSeconds = 0.05f;

        [Header("State Timing")]
        [SerializeField] private float crackedVisualHoldSeconds = 0f;
        [SerializeField] private float brokenAutoRestoreSeconds = -1f;

        [Header("Collision")]
        [SerializeField] private bool disableCollisionWhenBroken = true;
        [SerializeField] private Collider2D[] supportColliders;

        [Header("Visual Hooks")]
        [SerializeField] private GameObject intactVisualRoot;
        [SerializeField] private GameObject crackedVisualRoot;
        [SerializeField] private GameObject brokenVisualRoot;

        private DestructibleTileDurabilityModel _durability;
        private float _stateElapsedSeconds;
        private float _damageLockoutRemaining;

        public event Action<DestructibleTile, DestructibleTileState> StateChanged;
        public event Action<DestructibleTile> Damaged;

        public DestructibleTileState CurrentState => _durability != null ? _durability.State : DestructibleTileState.Intact;
        public int CurrentDurability => _durability != null ? _durability.CurrentDurability : maxDurability;
        public bool IsBroken => CurrentState == DestructibleTileState.Broken;

        private void Awake()
        {
            _durability = new DestructibleTileDurabilityModel(maxDurability);

            if (supportColliders == null || supportColliders.Length == 0)
            {
                supportColliders = GetComponents<Collider2D>();
            }

            RefreshPresentation();
        }

        private void Update()
        {
            _stateElapsedSeconds += Time.deltaTime;

            if (_damageLockoutRemaining > 0f)
            {
                _damageLockoutRemaining -= Time.deltaTime;
                if (_damageLockoutRemaining < 0f)
                {
                    _damageLockoutRemaining = 0f;
                }
            }

            if (CurrentState == DestructibleTileState.Broken &&
                brokenAutoRestoreSeconds >= 0f &&
                _stateElapsedSeconds >= brokenAutoRestoreSeconds)
            {
                RestoreToIntact();
            }
        }

        public bool ApplyDamage(int amount)
        {
            if (_durability == null || amount <= 0 || _damageLockoutRemaining > 0f)
            {
                return false;
            }

            var previousState = _durability.State;
            var accepted = _durability.ApplyDamage(amount);
            if (!accepted)
            {
                return false;
            }

            _damageLockoutRemaining = damageLockoutSeconds;
            Damaged?.Invoke(this);

            if (previousState != _durability.State)
            {
                _stateElapsedSeconds = 0f;
                RefreshPresentation();
                StateChanged?.Invoke(this, _durability.State);
            }
            else if (_durability.State == DestructibleTileState.Cracked)
            {
                // A no-op state change still refreshes visuals so placeholder art stays in sync.
                RefreshPresentation();
            }

            return true;
        }

        public void RestoreToIntact()
        {
            if (_durability == null)
            {
                return;
            }

            _durability.Restore();
            _stateElapsedSeconds = 0f;
            _damageLockoutRemaining = 0f;
            RefreshPresentation();
            StateChanged?.Invoke(this, _durability.State);
        }

        private void RefreshPresentation()
        {
            var state = _durability.State;

            var showCrackedVisual = state == DestructibleTileState.Cracked || (state == DestructibleTileState.Broken && _stateElapsedSeconds < crackedVisualHoldSeconds);

            SetActive(intactVisualRoot, state == DestructibleTileState.Intact);
            SetActive(crackedVisualRoot, showCrackedVisual);
            SetActive(brokenVisualRoot, state == DestructibleTileState.Broken);

            var colliderEnabled = !(disableCollisionWhenBroken && state == DestructibleTileState.Broken);
            for (var i = 0; i < supportColliders.Length; i++)
            {
                if (supportColliders[i] != null)
                {
                    supportColliders[i].enabled = colliderEnabled;
                }
            }
        }

        private static void SetActive(GameObject target, bool value)
        {
            if (target != null)
            {
                target.SetActive(value);
            }
        }
    }
}
