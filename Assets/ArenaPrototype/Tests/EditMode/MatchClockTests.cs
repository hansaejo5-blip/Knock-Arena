using ArenaPrototype.Runtime.Match;
using NUnit.Framework;

namespace ArenaPrototype.Tests.EditMode
{
    public sealed class MatchClockTests
    {
        [Test]
        public void Advance_ClampsAtZero()
        {
            var clock = new MatchClock(2f);

            clock.Advance(3f);

            Assert.That(clock.RemainingSeconds, Is.EqualTo(0f));
            Assert.That(clock.IsExpired, Is.True);
        }

        [Test]
        public void Advance_IgnoresNegativeDelta()
        {
            var clock = new MatchClock(10f);

            clock.Advance(-1f);

            Assert.That(clock.RemainingSeconds, Is.EqualTo(10f));
        }
    }
}
