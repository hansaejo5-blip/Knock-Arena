namespace Breakline.Runtime.Match
{
    public static class MatchResultCalculator
    {
        public static MatchResult Calculate(MatchScoreboard scoreboard)
        {
            var winner = scoreboard.ResolveWinnerOrTie();
            return new MatchResult(winner < 0, winner);
        }

        public static MatchResult Calculate(int playerOneScore, int playerTwoScore, MatchEndReason endReason)
        {
            if (playerOneScore == playerTwoScore)
            {
                return new MatchResult(true, -1, playerOneScore, playerTwoScore, endReason);
            }

            var winner = playerOneScore > playerTwoScore ? 0 : 1;
            return new MatchResult(false, winner, playerOneScore, playerTwoScore, endReason);
        }
    }
}
