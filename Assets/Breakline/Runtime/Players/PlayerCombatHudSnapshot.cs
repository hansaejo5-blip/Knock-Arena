namespace Breakline.Runtime.Players
{
    public readonly struct PlayerCombatHudSnapshot
    {
        public PlayerCombatHudSnapshot(bool dashReady, bool attackReady, float dashCooldownNormalized, float attackCooldownNormalized)
        {
            DashReady = dashReady;
            AttackReady = attackReady;
            DashCooldownNormalized = dashCooldownNormalized;
            AttackCooldownNormalized = attackCooldownNormalized;
        }

        public bool DashReady { get; }
        public bool AttackReady { get; }
        public float DashCooldownNormalized { get; }
        public float AttackCooldownNormalized { get; }
    }
}
