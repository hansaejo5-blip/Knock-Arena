namespace Breakline.Runtime.Tiles
{
    public sealed class TileDurabilityModel
    {
        public TileDurabilityModel(int maxDurability)
        {
            MaxDurability = maxDurability < 1 ? 1 : maxDurability;
            CurrentDurability = MaxDurability;
        }

        public int MaxDurability { get; }
        public int CurrentDurability { get; private set; }

        public BreaklineTileState State
        {
            get
            {
                if (CurrentDurability <= 0)
                {
                    return BreaklineTileState.Broken;
                }

                if (CurrentDurability < MaxDurability)
                {
                    return BreaklineTileState.Cracked;
                }

                return BreaklineTileState.Intact;
            }
        }

        public void ApplyDamage(int amount)
        {
            if (amount <= 0 || State == BreaklineTileState.Broken)
            {
                return;
            }

            CurrentDurability -= amount;
            if (CurrentDurability < 0)
            {
                CurrentDurability = 0;
            }
        }

        public void Restore()
        {
            CurrentDurability = MaxDurability;
        }
    }
}
