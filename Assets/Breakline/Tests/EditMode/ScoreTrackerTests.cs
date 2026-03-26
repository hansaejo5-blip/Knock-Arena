using Breakline.Runtime.Match;
using NUnit.Framework;
using UnityEngine;

namespace Breakline.Tests.EditMode
{
    public sealed class ScoreTrackerTests
    {
        [Test]
        public void AddRingOutPoint_IncrementsExpectedPlayer()
        {
            var gameObject = new GameObject("ScoreTracker");
            var tracker = gameObject.AddComponent<ScoreTracker>();
            tracker.Configure(2);

            tracker.AddRingOutPoint(1);

            Assert.That(tracker.GetScore(0), Is.EqualTo(0));
            Assert.That(tracker.GetScore(1), Is.EqualTo(1));

            Object.DestroyImmediate(gameObject);
        }
    }
}
