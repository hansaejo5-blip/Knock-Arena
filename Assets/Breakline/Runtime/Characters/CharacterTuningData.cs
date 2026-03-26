using System;
using UnityEngine;

namespace Breakline.Runtime.Characters
{
    [Serializable]
    public struct CharacterTuningData
    {
        [Header("Core Combat")]
        [Tooltip("Base horizontal movement speed for the shared PlayerController.")]
        public float movementSpeed;

        [Tooltip("How often the shared dash becomes available again.")]
        public float dashCooldownSeconds;

        [Tooltip("Forward attack reach. This is applied to the shared AttackHitbox offset.")]
        public float attackRange;

        [Tooltip("Horizontal knockback force applied by the shared basic attack.")]
        public float attackForce;

        [Tooltip("How much floor damage the shared attack deals per hit.")]
        public int tileDamage;
    }
}
