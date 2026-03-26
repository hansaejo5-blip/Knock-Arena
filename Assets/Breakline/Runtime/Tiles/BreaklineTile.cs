using System;
using Breakline.Runtime.Config;
using UnityEngine;

namespace Breakline.Runtime.Tiles
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class BreaklineTile : MonoBehaviour
    {
        [SerializeField] private BreaklineTileSettings settings;
        [SerializeField] private Renderer intactRenderer;
        [SerializeField] private Renderer crackedRenderer;
        [SerializeField] private Renderer brokenRenderer;

        private Collider2D[] _colliders;
        private TileDurabilityModel _durability;

        public event Action<BreaklineTileState> StateChanged;

        public BreaklineTileState CurrentState => _durability != null ? _durability.State : BreaklineTileState.Intact;

        private void Awake()
        {
            _colliders = GetComponentsInChildren<Collider2D>(true);
            _durability = new TileDurabilityModel(settings != null ? settings.maxDurability : 2);
            RefreshVisuals();
        }

        public void ApplyDamage(int damage)
        {
            _durability.ApplyDamage(damage);
            RefreshVisuals();
            StateChanged?.Invoke(_durability.State);
        }

        public void Restore()
        {
            _durability.Restore();
            RefreshVisuals();
            StateChanged?.Invoke(_durability.State);
        }

        private void RefreshVisuals()
        {
            var state = _durability.State;
            SetRenderer(intactRenderer, state == BreaklineTileState.Intact);
            SetRenderer(crackedRenderer, state == BreaklineTileState.Cracked);
            SetRenderer(brokenRenderer, state == BreaklineTileState.Broken);

            var collidersEnabled = state != BreaklineTileState.Broken;
            for (var i = 0; i < _colliders.Length; i++)
            {
                _colliders[i].enabled = collidersEnabled;
            }
        }

        private static void SetRenderer(Renderer renderer, bool visible)
        {
            if (renderer != null)
            {
                renderer.enabled = visible;
            }
        }
    }
}
