using Breakline.Runtime.Match;
using UnityEngine;
using UnityEngine.UI;

namespace Breakline.Runtime.Hud
{
    // Attach this to a Canvas GameObject and wire TMP labels / banner roots in the Inspector.
    public sealed class BreaklineHudController : MonoBehaviour
    {
        [Header("Sources")]
        [SerializeField] private BreaklineHudBridge hudBridge;

        [Header("Timer")]
        [SerializeField] private Text timerText;

        [Header("Scores")]
        [SerializeField] private Text playerScoreText;
        [SerializeField] private Text enemyScoreText;

        [Header("Result Banner")]
        [SerializeField] private GameObject resultBannerRoot;
        [SerializeField] private Text resultBannerText;

        private void OnEnable()
        {
            if (hudBridge != null)
            {
                hudBridge.SnapshotPublished += OnSnapshotPublished;
                OnSnapshotPublished(hudBridge.Current);
            }
        }

        private void OnDisable()
        {
            if (hudBridge != null)
            {
                hudBridge.SnapshotPublished -= OnSnapshotPublished;
            }
        }

        private void OnSnapshotPublished(BreaklineHudSnapshot snapshot)
        {
            if (timerText != null)
            {
                timerText.text = FormatTimer(snapshot.RemainingSeconds);
            }

            if (playerScoreText != null)
            {
                playerScoreText.text = snapshot.PlayerOneScore.ToString();
            }

            if (enemyScoreText != null)
            {
                enemyScoreText.text = snapshot.PlayerTwoScore.ToString();
            }

            if (resultBannerRoot != null)
            {
                resultBannerRoot.SetActive(snapshot.MatchComplete);
            }

            if (resultBannerText != null)
            {
                resultBannerText.text = BuildResultBanner(snapshot);
            }
        }

        private static string FormatTimer(float remainingSeconds)
        {
            var safeSeconds = Mathf.Max(0f, remainingSeconds);
            var minutes = Mathf.FloorToInt(safeSeconds / 60f);
            var seconds = Mathf.FloorToInt(safeSeconds % 60f);
            return $"{minutes:00}:{seconds:00}";
        }

        private static string BuildResultBanner(BreaklineHudSnapshot snapshot)
        {
            if (!snapshot.MatchComplete)
            {
                return string.Empty;
            }

            if (snapshot.WinnerPlayerIndex < 0)
            {
                return "DRAW";
            }

            return snapshot.WinnerPlayerIndex == 0 ? "YOU WIN" : "YOU LOSE";
        }
    }
}
