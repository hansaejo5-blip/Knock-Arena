namespace Breakline.Runtime.Match
{
    public struct TrophyResult
    {
        public TrophyResult(string gradeId, int trophyDelta, RankTierId rankTierId = RankTierId.Bronze)
        {
            GradeId = gradeId;
            TrophyDelta = trophyDelta;
            RankTierId = rankTierId;
        }

        public string GradeId { get; private set; }
        public int TrophyDelta { get; private set; }
        public RankTierId RankTierId { get; private set; }
    }
}
