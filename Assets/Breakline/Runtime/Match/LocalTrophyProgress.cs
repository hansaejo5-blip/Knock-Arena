using UnityEngine;

namespace Breakline.Runtime.Match
{
    // Local-only prototype storage. Keeps trophy progress in memory for the current play session.
    public sealed class LocalTrophyProgress : MonoBehaviour
    {
        [SerializeField] private int startingTrophies;
        [SerializeField] private bool enableWinStreakBonus;

        public int CurrentTrophies { get; private set; }
        public int CurrentWinStreak { get; private set; }
        public bool EnableWinStreakBonus => enableWinStreakBonus;

        private void Awake()
        {
            CurrentTrophies = Mathf.Max(0, startingTrophies);
            CurrentWinStreak = 0;
        }

        public ResultSummary ApplyMatchResult(MatchResult result, int localPlayerIndex = 0)
        {
            var summary = ResultSummaryGenerator.Create(
                result,
                CurrentTrophies,
                localPlayerIndex,
                enableWinStreakBonus,
                CurrentWinStreak);

            CurrentTrophies = summary.NewTrophies;

            if (result.IsTie)
            {
                CurrentWinStreak = 0;
            }
            else if (result.WinnerPlayerIndex == localPlayerIndex)
            {
                CurrentWinStreak++;
            }
            else
            {
                CurrentWinStreak = 0;
            }

            return summary;
        }
    }
}
