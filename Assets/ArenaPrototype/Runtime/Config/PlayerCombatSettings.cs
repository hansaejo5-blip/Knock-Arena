using UnityEngine;

namespace ArenaPrototype.Runtime.Config
{
    [CreateAssetMenu(
        fileName = "PlayerCombatSettings",
        menuName = "Arena Prototype/Config/Player Combat Settings")]
    public sealed class PlayerCombatSettings : ScriptableObject
    {
        [Header("Movement")]
        [Min(0f)]
        public float moveSpeed = 8f;

        [Min(0f)]
        public float jumpImpulse = 13f;

        [Min(0)]
        public int extraAirJumps = 0;

        [Header("Ground Check")]
        public TransformAnchor groundCheck = TransformAnchor.Feet;
        public Vector2 groundCheckOffset = new(0f, -0.7f);
        [Min(0.01f)]
        public float groundCheckRadius = 0.18f;

        [Header("Shove")]
        [Min(0f)]
        public float shoveRange = 1.15f;

        [Min(0f)]
        public float shoveForce = 12f;

        [Min(0f)]
        public float shoveUpForce = 2.5f;

        [Min(0f)]
        public float shoveCooldownSeconds = 0.45f;

        [Header("Floor Break")]
        [Min(0f)]
        public float breakRange = 1.05f;

        [Min(0f)]
        public float breakCooldownSeconds = 0.3f;

        [Min(0.01f)]
        public float breakProbeRadius = 0.2f;
    }

    public enum TransformAnchor
    {
        Center = 0,
        Feet = 1
    }
}
