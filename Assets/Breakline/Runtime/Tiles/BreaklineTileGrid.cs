using UnityEngine;

namespace Breakline.Runtime.Tiles
{
    public sealed class BreaklineTileGrid : MonoBehaviour
    {
        [SerializeField] private BreaklineTile tilePrefab;
        [SerializeField] private Transform tileRoot;
        [SerializeField] private Vector2Int gridSize = new(10, 4);
        [SerializeField] private Vector2 tileSpacing = new(1f, 1f);
        [SerializeField] private Vector2 originOffset = new(-4.5f, -1.5f);

        public void RebuildImmediate()
        {
            if (tilePrefab == null)
            {
                Debug.LogWarning("BreaklineTileGrid requires a tile prefab.", this);
                return;
            }

            var root = tileRoot != null ? tileRoot : transform;
            Clear(root);

            for (var y = 0; y < gridSize.y; y++)
            {
                for (var x = 0; x < gridSize.x; x++)
                {
                    var tile = Instantiate(tilePrefab, root);
                    tile.name = $"Tile_{x}_{y}";
                    tile.transform.localPosition = new Vector3(originOffset.x + (x * tileSpacing.x), originOffset.y + (y * tileSpacing.y), 0f);
                }
            }
        }

        private void Clear(Transform root)
        {
            for (var i = root.childCount - 1; i >= 0; i--)
            {
                var child = root.GetChild(i).gameObject;
                if (Application.isPlaying)
                {
                    Destroy(child);
                }
                else
                {
                    DestroyImmediate(child);
                }
            }
        }
    }
}
