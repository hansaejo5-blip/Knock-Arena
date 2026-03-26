using Breakline.Runtime.Match;
using NUnit.Framework;

namespace Breakline.Tests.EditMode
{
    public sealed class TrophyCalculatorTests
    {
        [Test]
        public void CalculateDelta_ReturnsPlusThirty_OnWin()
        {
            var result = new MatchResult(false, 0, 3, 1, MatchEndReason.TimeExpired);

            var delta = TrophyCalculator.CalculateDelta(result);

            Assert.That(delta, Is.EqualTo(30));
        }

        [Test]
        public void CalculateDelta_ReturnsMinusTwenty_OnLoss()
        {
            var result = new MatchResult(false, 1, 1, 3, MatchEndReason.TimeExpired);

            var delta = TrophyCalculator.CalculateDelta(result);

            Assert.That(delta, Is.EqualTo(-20));
        }

        [Test]
        public void CalculateDelta_ReturnsZero_OnDraw()
        {
            var result = new MatchResult(true, -1, 2, 2, MatchEndReason.TimeExpired);

            var delta = TrophyCalculator.CalculateDelta(result);

            Assert.That(delta, Is.EqualTo(0));
        }

        [Test]
        public void ResultSummary_ResolvesExpectedTierThresholds()
        {
            var result = new MatchResult(false, 0, 4, 1, MatchEndReason.TimeExpired);
            var summary = ResultSummaryGenerator.Create(result, 490);

            Assert.That(summary.PreviousTier.TierId, Is.EqualTo(RankTierId.Silver));
            Assert.That(summary.NewTrophies, Is.EqualTo(520));
            Assert.That(summary.NewTier.TierId, Is.EqualTo(RankTierId.Gold));
        }

        [Test]
        public void ResultSummary_AppliesWinStreakBonus_WhenEnabled()
        {
            var result = new MatchResult(false, 0, 3, 1, MatchEndReason.TimeExpired);
            var summary = ResultSummaryGenerator.Create(result, 200, 0, true, 4);

            Assert.That(summary.TrophyDelta, Is.EqualTo(34));
            Assert.That(summary.NewTrophies, Is.EqualTo(234));
        }
    }
}
