using Breakline.Runtime.Match;
using NUnit.Framework;

namespace Breakline.Tests.EditMode
{
    public sealed class MatchResultCalculatorTests
    {
        [Test]
        public void Calculate_ReturnsWinner_WhenOnePlayerLeads()
        {
            var scoreboard = new MatchScoreboard(2);
            scoreboard.AddPoint(1);
            scoreboard.AddPoint(1);

            var result = MatchResultCalculator.Calculate(scoreboard);

            Assert.That(result.IsTie, Is.False);
            Assert.That(result.WinnerPlayerIndex, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_ReturnsTie_WhenScoresMatch()
        {
            var scoreboard = new MatchScoreboard(2);
            scoreboard.AddPoint(0);
            scoreboard.AddPoint(1);

            var result = MatchResultCalculator.Calculate(scoreboard);

            Assert.That(result.IsTie, Is.True);
            Assert.That(result.WinnerPlayerIndex, Is.EqualTo(-1));
        }
    }
}
