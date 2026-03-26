using UnityEngine;

namespace ArenaPrototype.Runtime.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class FloorTile : MonoBehaviour
    {
        [SerializeField] private Vector2Int gridPosition;

        private Collider2D[] _colliders;
        private Renderer[] _renderers;

        public Vector2Int GridPosition => gridPosition;
        public bool IsBroken { get; private set; }

        private void Awake()
        {
            CacheComponents();
        }

        public void Initialize(Vector2Int position)
        {
            gridPosition = position;
            CacheComponents();
            Restore();
        }

        public void Break()
        {
            if (IsBroken)
            {
                return;
            }

            IsBroken = true;
            SetVisualState(false);
        }

        public void Restore()
        {
            IsBroken = false;
            SetVisualState(true);
        }

        private void CacheComponents()
        {
            _colliders ??= GetComponentsInChildren<Collider2D>(true);
            _renderers ??= GetComponentsInChildren<Renderer>(true);
        }

        private void SetVisualState(bool enabled)
        {
            CacheComponents();

            for (var i = 0; i < _colliders.Length; i++)
            {
                _colliders[i].enabled = enabled;
            }

            for (var i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].enabled = enabled;
            }
        }
    }
}
