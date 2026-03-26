namespace Breakline.Runtime.Match
{
    public static class ResultSummaryGenerator
    {
        public static ResultSummary Create(MatchResult result, int currentTrophies, int localPlayerIndex = 0, bool enableWinStreakBonus = false, int currentWinStreak = 0)
        {
            var previousTrophies = currentTrophies < 0 ? 0 : currentTrophies;
            var delta = TrophyCalculator.CalculateDelta(result, localPlayerIndex, enableWinStreakBonus, currentWinStreak);
            var newTrophies = previousTrophies + delta;
            if (newTrophies < 0)
            {
                newTrophies = 0;
            }

            var previousTier = RankTierCatalog.ResolveTier(previousTrophies);
            var newTier = RankTierCatalog.ResolveTier(newTrophies);

            return new ResultSummary(
                result,
                delta,
                previousTrophies,
                newTrophies,
                previousTier,
                newTier,
                BuildHeadline(result, localPlayerIndex),
                localPlayerIndex);
        }

        private static string BuildHeadline(MatchResult result, int localPlayerIndex)
        {
            if (result.IsTie)
            {
                return "Draw";
            }

            return result.WinnerPlayerIndex == localPlayerIndex ? "Victory" : "Defeat";
        }
    }
}
