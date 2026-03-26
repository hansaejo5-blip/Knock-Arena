using UnityEngine;

namespace Breakline.Runtime.Combat
{
    public readonly struct CombatHitData
    {
        public CombatHitData(Vector2 hitOrigin, int attackerFacingSign, float horizontalKnockback, float verticalKnockback, float stunDurationSeconds)
        {
            HitOrigin = hitOrigin;
            AttackerFacingSign = attackerFacingSign;
            HorizontalKnockback = horizontalKnockback;
            VerticalKnockback = verticalKnockback;
            StunDurationSeconds = stunDurationSeconds;
        }

        public Vector2 HitOrigin { get; }
        public int AttackerFacingSign { get; }
        public float HorizontalKnockback { get; }
        public float VerticalKnockback { get; }
        public float StunDurationSeconds { get; }
    }
}
