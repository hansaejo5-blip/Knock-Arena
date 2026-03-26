using System;

namespace Breakline.Runtime.Match
{
    [Serializable]
    public struct RankTierDefinition
    {
        public RankTierId TierId;
        public int MinimumTrophies;
        public string DisplayName;

        public RankTierDefinition(RankTierId tierId, int minimumTrophies, string displayName)
        {
            TierId = tierId;
            MinimumTrophies = minimumTrophies;
            DisplayName = displayName;
        }
    }
}
