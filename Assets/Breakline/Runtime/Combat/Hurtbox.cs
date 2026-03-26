using Breakline.Runtime.Players;
using UnityEngine;

namespace Breakline.Runtime.Combat
{
    // Add this to the same GameObject as the player's main body collider, or to a child hit target.
    // Set the collider to a non-trigger if you want body physics, or to a trigger if you use a separate hurtbox volume.
    public sealed class Hurtbox : MonoBehaviour
    {
        [SerializeField] private PlayerController owner;
        [SerializeField] private Transform hitPoint;

        public PlayerController Owner => owner;
        public Vector2 HitPoint => hitPoint != null ? hitPoint.position : transform.position;

        public bool TryReceiveHit(PlayerController attacker, CombatHitData hitData)
        {
            if (owner == null || attacker == null || attacker == owner)
            {
                return false;
            }

            owner.ReceiveHit(attacker, hitData, HitPoint);
            return true;
        }
    }
}
