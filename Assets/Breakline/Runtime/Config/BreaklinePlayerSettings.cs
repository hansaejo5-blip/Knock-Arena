using UnityEngine;

namespace Breakline.Runtime.Config
{
    [CreateAssetMenu(fileName = "BreaklinePlayerSettings", menuName = "Breakline/Config/Player Settings")]
    public sealed class BreaklinePlayerSettings : ScriptableObject
    {
        [Min(0f)] public float moveSpeed = 8f;
        [Min(0f)] public float jumpImpulse = 13f;
        [Min(0f)] public float maxHorizontalSpeed = 8f;
        public Vector2 groundCheckOffset = new(0f, -0.7f);
        [Min(0.01f)] public float groundCheckRadius = 0.18f;
        [Min(0f)] public float attackRange = 1f;
        [Min(0f)] public float attackRadius = 0.35f;
        [Min(0f)] public float attackCooldownSeconds = 0.3f;
        [Min(0)] public int tileDamagePerHit = 1;
        [Min(0f)] public float pushForce = 11f;
        [Min(0f)] public float pushUpForce = 2f;
    }
}
