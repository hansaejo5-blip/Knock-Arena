using System;
using System.Collections.Generic;
using UnityEngine;

namespace Breakline.Runtime.Tiles
{
    public sealed class DestructibleTileManager : MonoBehaviour
    {
        [SerializeField] private List<DestructibleTile> tiles = new();
        [SerializeField] private bool autoCollectChildrenOnAwake = true;

        public event Action<DestructibleTileDebugSnapshot> TileSummaryChanged;

        public IReadOnlyList<DestructibleTile> Tiles => tiles;

        private void Awake()
        {
            if (autoCollectChildrenOnAwake)
            {
                RefreshTileList();
            }

            PublishSummary();
        }

        private void OnEnable()
        {
            SubscribeToTiles();
            PublishSummary();
        }

        private void OnDisable()
        {
            UnsubscribeFromTiles();
        }

        [ContextMenu("Refresh Tile List")]
        public void RefreshTileList()
        {
            UnsubscribeFromTiles();
            tiles.Clear();
            var found = GetComponentsInChildren<DestructibleTile>(true);
            for (var i = 0; i < found.Length; i++)
            {
                tiles.Add(found[i]);
            }

            SubscribeToTiles();
            PublishSummary();
        }

        [ContextMenu("Restore All Tiles")]
        public void RestoreAllTiles()
        {
            for (var i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] != null)
                {
                    tiles[i].RestoreToIntact();
                }
            }

            PublishSummary();
        }

        private void SubscribeToTiles()
        {
            for (var i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] != null)
                {
                    tiles[i].StateChanged -= OnTileStateChanged;
                    tiles[i].StateChanged += OnTileStateChanged;
                }
            }
        }

        private void UnsubscribeFromTiles()
        {
            for (var i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] != null)
                {
                    tiles[i].StateChanged -= OnTileStateChanged;
                }
            }
        }

        private void OnTileStateChanged(DestructibleTile _, DestructibleTileState __)
        {
            PublishSummary();
        }

        private void PublishSummary()
        {
            var intact = 0;
            var cracked = 0;
            var broken = 0;

            for (var i = 0; i < tiles.Count; i++)
            {
                var tile = tiles[i];
                if (tile == null)
                {
                    continue;
                }

                switch (tile.CurrentState)
                {
                    case DestructibleTileState.Intact:
                        intact++;
                        break;
                    case DestructibleTileState.Cracked:
                        cracked++;
                        break;
                    case DestructibleTileState.Broken:
                        broken++;
                        break;
                }
            }

            TileSummaryChanged?.Invoke(new DestructibleTileDebugSnapshot(intact, cracked, broken));
        }
    }
}
