namespace Breakline.Runtime.Match
{
    public static class RankTierCatalog
    {
        // Example prototype thresholds. Designers can replace this with an asset later if needed.
        public static readonly RankTierDefinition[] DefaultTiers =
        {
            new RankTierDefinition(RankTierId.Bronze, 0, "Bronze"),
            new RankTierDefinition(RankTierId.Silver, 200, "Silver"),
            new RankTierDefinition(RankTierId.Gold, 500, "Gold"),
            new RankTierDefinition(RankTierId.Platinum, 900, "Platinum"),
            new RankTierDefinition(RankTierId.Diamond, 1400, "Diamond")
        };

        public static RankTierDefinition ResolveTier(int trophies)
        {
            var safeTrophies = trophies < 0 ? 0 : trophies;
            var resolved = DefaultTiers[0];

            for (var i = 0; i < DefaultTiers.Length; i++)
            {
                if (safeTrophies >= DefaultTiers[i].MinimumTrophies)
                {
                    resolved = DefaultTiers[i];
                }
            }

            return resolved;
        }
    }
}
