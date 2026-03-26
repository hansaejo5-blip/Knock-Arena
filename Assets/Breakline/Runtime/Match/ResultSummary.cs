namespace Breakline.Runtime.Match
{
    public struct ResultSummary
    {
        public ResultSummary(
            MatchResult matchResult,
            int trophyDelta,
            int previousTrophies,
            int newTrophies,
            RankTierDefinition previousTier,
            RankTierDefinition newTier,
            string headline,
            int localPlayerIndex)
        {
            MatchResult = matchResult;
            TrophyDelta = trophyDelta;
            PreviousTrophies = previousTrophies;
            NewTrophies = newTrophies;
            PreviousTier = previousTier;
            NewTier = newTier;
            Headline = headline;
            LocalPlayerIndex = localPlayerIndex;
        }

        public MatchResult MatchResult { get; private set; }
        public int TrophyDelta { get; private set; }
        public int PreviousTrophies { get; private set; }
        public int NewTrophies { get; private set; }
        public RankTierDefinition PreviousTier { get; private set; }
        public RankTierDefinition NewTier { get; private set; }
        public string Headline { get; private set; }
        public int LocalPlayerIndex { get; private set; }
        public bool RankChanged => PreviousTier.TierId != NewTier.TierId;
    }
}
