namespace Breakline.Runtime.Match
{
    public static class TrophyResultCalculator
    {
        public static TrophyResult CalculatePlaceholder(MatchResult result)
        {
            var summary = ResultSummaryGenerator.Create(result, 0);
            return new TrophyResult(summary.Headline, summary.TrophyDelta, summary.NewTier.TierId);
        }
    }
}
