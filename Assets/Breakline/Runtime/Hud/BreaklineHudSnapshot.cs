using Breakline.Runtime.Match;

namespace Breakline.Runtime.Hud
{
    public readonly struct BreaklineHudSnapshot
    {
        public BreaklineHudSnapshot(float remainingSeconds, int playerOneScore, int playerTwoScore, bool matchComplete, int winnerPlayerIndex, string trophyGradeId, int trophyDelta, MatchPhase phase = MatchPhase.WaitingToStart)
        {
            RemainingSeconds = remainingSeconds;
            PlayerOneScore = playerOneScore;
            PlayerTwoScore = playerTwoScore;
            MatchComplete = matchComplete;
            WinnerPlayerIndex = winnerPlayerIndex;
            TrophyGradeId = trophyGradeId;
            TrophyDelta = trophyDelta;
            Phase = phase;
        }

        public float RemainingSeconds { get; }
        public int PlayerOneScore { get; }
        public int PlayerTwoScore { get; }
        public bool MatchComplete { get; }
        public int WinnerPlayerIndex { get; }
        public string TrophyGradeId { get; }
        public int TrophyDelta { get; }
        public MatchPhase Phase { get; }
    }
}
