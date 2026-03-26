namespace Breakline.Runtime.Match
{
    public struct MatchResult
    {
        public MatchResult(bool isTie, int winnerPlayerIndex)
            : this(isTie, winnerPlayerIndex, 0, 0, MatchEndReason.TimeExpired)
        {
        }

        public MatchResult(bool isTie, int winnerPlayerIndex, int playerOneScore, int playerTwoScore, MatchEndReason endReason)
        {
            IsTie = isTie;
            WinnerPlayerIndex = winnerPlayerIndex;
            PlayerOneScore = playerOneScore;
            PlayerTwoScore = playerTwoScore;
            EndReason = endReason;
        }

        public bool IsTie { get; private set; }
        public int WinnerPlayerIndex { get; private set; }
        public int PlayerOneScore { get; private set; }
        public int PlayerTwoScore { get; private set; }
        public MatchEndReason EndReason { get; private set; }
    }
}
