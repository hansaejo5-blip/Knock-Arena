namespace Breakline.Runtime.Match
{
    public static class TrophyCalculator
    {
        public const int WinTrophyDelta = 30;
        public const int LossTrophyDelta = -20;
        public const int DrawTrophyDelta = 0;

        public static int CalculateDelta(MatchResult result, int localPlayerIndex = 0, bool enableWinStreakBonus = false, int currentWinStreak = 0)
        {
            var delta = result.IsTie
                ? DrawTrophyDelta
                : result.WinnerPlayerIndex == localPlayerIndex
                    ? WinTrophyDelta
                    : LossTrophyDelta;

            if (enableWinStreakBonus && currentWinStreak > 0 && result.WinnerPlayerIndex == localPlayerIndex)
            {
                delta += currentWinStreak;
            }

            return delta;
        }
    }
}
