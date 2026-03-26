namespace Breakline.Runtime.Tiles
{
    public sealed class DestructibleTileDurabilityModel
    {
        public DestructibleTileDurabilityModel(int maxDurability)
        {
            MaxDurability = maxDurability < 1 ? 1 : maxDurability;
            CurrentDurability = MaxDurability;
        }

        public int MaxDurability { get; }
        public int CurrentDurability { get; private set; }

        public DestructibleTileState State
        {
            get
            {
                if (CurrentDurability <= 0)
                {
                    return DestructibleTileState.Broken;
                }

                if (CurrentDurability < MaxDurability)
                {
                    return DestructibleTileState.Cracked;
                }

                return DestructibleTileState.Intact;
            }
        }

        public bool ApplyDamage(int amount)
        {
            if (amount <= 0 || State == DestructibleTileState.Broken)
            {
                return false;
            }

            CurrentDurability -= amount;
            if (CurrentDurability < 0)
            {
                CurrentDurability = 0;
            }

            return true;
        }

        public void Restore()
        {
            CurrentDurability = MaxDurability;
        }
    }
}
