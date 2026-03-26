using ArenaPrototype.Runtime.Match;
using NUnit.Framework;

namespace ArenaPrototype.Tests.EditMode
{
    public sealed class MatchScoreboardTests
    {
        [Test]
        public void GetLeader_ReturnsLeader_WhenOnePlayerLeads()
        {
            var scoreboard = new MatchScoreboard(2);
            scoreboard.AddPoint(1);
            scoreboard.AddPoint(1);
            scoreboard.AddPoint(0);

            Assert.That(scoreboard.GetLeader(), Is.EqualTo(1));
            Assert.That(scoreboard.GetScore(1), Is.EqualTo(2));
        }

        [Test]
        public void GetLeader_ReturnsTieFlag_WhenScoresMatch()
        {
            var scoreboard = new MatchScoreboard(2);
            scoreboard.AddPoint(0);
            scoreboard.AddPoint(1);

            Assert.That(scoreboard.GetLeader(), Is.EqualTo(-1));
        }
    }
}
