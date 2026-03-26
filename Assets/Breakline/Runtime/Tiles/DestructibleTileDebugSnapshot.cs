namespace Breakline.Runtime.Tiles
{
    public readonly struct DestructibleTileDebugSnapshot
    {
        public DestructibleTileDebugSnapshot(int intactCount, int crackedCount, int brokenCount)
        {
            IntactCount = intactCount;
            CrackedCount = crackedCount;
            BrokenCount = brokenCount;
        }

        public int IntactCount { get; }
        public int CrackedCount { get; }
        public int BrokenCount { get; }
    }
}
