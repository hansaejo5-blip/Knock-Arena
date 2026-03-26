using UnityEngine;

namespace Breakline.Runtime.Combat
{
    public static class KnockbackResolver
    {
        public static Vector2 ResolveImpulse(CombatHitData hitData, Vector2 targetPosition)
        {
            var horizontalSign = hitData.AttackerFacingSign;
            if (horizontalSign == 0)
            {
                horizontalSign = targetPosition.x >= hitData.HitOrigin.x ? 1 : -1;
            }

            return new Vector2(horizontalSign * hitData.HorizontalKnockback, hitData.VerticalKnockback);
        }
    }
}
