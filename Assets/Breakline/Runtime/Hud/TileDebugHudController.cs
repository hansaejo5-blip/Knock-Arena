using Breakline.Runtime.Tiles;
using UnityEngine;
using UnityEngine.UI;

namespace Breakline.Runtime.Hud
{
    // Optional debug readout. Useful while tuning how fast the arena disappears.
    public sealed class TileDebugHudController : MonoBehaviour
    {
        [SerializeField] private DestructibleTileManager tileManager;
        [SerializeField] private Text tileStateText;

        private void OnEnable()
        {
            if (tileManager != null)
            {
                tileManager.TileSummaryChanged += OnTileSummaryChanged;
                tileManager.RefreshTileList();
            }
        }

        private void OnDisable()
        {
            if (tileManager != null)
            {
                tileManager.TileSummaryChanged -= OnTileSummaryChanged;
            }
        }

        private void OnTileSummaryChanged(DestructibleTileDebugSnapshot snapshot)
        {
            if (tileStateText == null)
            {
                return;
            }

            tileStateText.text = $"Tiles  I:{snapshot.IntactCount}  C:{snapshot.CrackedCount}  B:{snapshot.BrokenCount}";
        }
    }
}
