using Breakline.Runtime.Tiles;
using NUnit.Framework;

namespace Breakline.Tests.EditMode
{
    public sealed class TileDurabilityModelTests
    {
        [Test]
        public void ApplyDamage_TransitionsIntactToCrackedToBroken()
        {
            var model = new TileDurabilityModel(2);

            Assert.That(model.State, Is.EqualTo(BreaklineTileState.Intact));
            model.ApplyDamage(1);
            Assert.That(model.State, Is.EqualTo(BreaklineTileState.Cracked));
            model.ApplyDamage(1);
            Assert.That(model.State, Is.EqualTo(BreaklineTileState.Broken));
        }
    }
}
