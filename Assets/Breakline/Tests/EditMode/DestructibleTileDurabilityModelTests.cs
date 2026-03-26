using Breakline.Runtime.Tiles;
using NUnit.Framework;

namespace Breakline.Tests.EditMode
{
    public sealed class DestructibleTileDurabilityModelTests
    {
        [Test]
        public void ApplyDamage_TransitionsThroughThreeStates()
        {
            var model = new DestructibleTileDurabilityModel(2);

            Assert.That(model.State, Is.EqualTo(DestructibleTileState.Intact));

            model.ApplyDamage(1);
            Assert.That(model.State, Is.EqualTo(DestructibleTileState.Cracked));

            model.ApplyDamage(1);
            Assert.That(model.State, Is.EqualTo(DestructibleTileState.Broken));
        }

        [Test]
        public void Restore_ReturnsToIntact()
        {
            var model = new DestructibleTileDurabilityModel(2);
            model.ApplyDamage(2);

            model.Restore();

            Assert.That(model.State, Is.EqualTo(DestructibleTileState.Intact));
            Assert.That(model.CurrentDurability, Is.EqualTo(2));
        }
    }
}
