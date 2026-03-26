using Breakline.Runtime.Match;
using NUnit.Framework;

namespace Breakline.Tests.EditMode
{
    public sealed class MatchResultRulesTests
    {
        [Test]
        public void Calculate_ReturnsDraw_WhenScoresTie()
        {
            var result = MatchResultCalculator.Calculate(2, 2, MatchEndReason.TimeExpired);

            Assert.That(result.IsTie, Is.True);
            Assert.That(result.WinnerPlayerIndex, Is.EqualTo(-1));
            Assert.That(result.PlayerOneScore, Is.EqualTo(2));
            Assert.That(result.PlayerTwoScore, Is.EqualTo(2));
        }

        [Test]
        public void Calculate_ReturnsHigherScoreWinner_WhenTimeEnds()
        {
            var result = MatchResultCalculator.Calculate(3, 1, MatchEndReason.TimeExpired);

            Assert.That(result.IsTie, Is.False);
            Assert.That(result.WinnerPlayerIndex, Is.EqualTo(0));
        }
    }
}
