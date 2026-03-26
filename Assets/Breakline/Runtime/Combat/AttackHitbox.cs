using System.Collections.Generic;
using Breakline.Runtime.Players;
using Breakline.Runtime.Tiles;
using UnityEngine;

namespace Breakline.Runtime.Combat
{
    // Create a child GameObject under the player and attach this script there.
    // The hitbox uses OverlapBoxAll so it stays easy to debug and deterministic for a prototype.
    public sealed class AttackHitbox : MonoBehaviour
    {
        [SerializeField] private Vector2 boxSize = new(1.1f, 0.9f);
        [SerializeField] private Vector2 localOffset = new(0.8f, 0f);
        [SerializeField] private LayerMask hurtboxMask = ~0;
        [SerializeField] private LayerMask tileMask = ~0;
        [SerializeField] private int tileDamage = 1;

        private readonly HashSet<Hurtbox> _hitBuffer = new();
        private readonly HashSet<DestructibleTile> _tileHitBuffer = new();

        public float AttackRange => Mathf.Abs(localOffset.x);
        public int TileDamage => tileDamage;

        public int PerformHit(PlayerController attacker, int facingSign, CombatHitData hitData)
        {
            _hitBuffer.Clear();
            _tileHitBuffer.Clear();

            var worldCenter = GetWorldCenter(facingSign);
            var colliders = Physics2D.OverlapBoxAll(worldCenter, boxSize, 0f, hurtboxMask);
            var hitCount = 0;

            for (var i = 0; i < colliders.Length; i++)
            {
                var hurtbox = colliders[i].GetComponentInParent<Hurtbox>();
                if (hurtbox == null || _hitBuffer.Contains(hurtbox))
                {
                    continue;
                }

                _hitBuffer.Add(hurtbox);
                if (hurtbox.TryReceiveHit(attacker, hitData))
                {
                    hitCount++;
                }
            }

            DamageTiles(worldCenter);

            return hitCount;
        }

        public Vector2 GetWorldCenter(int facingSign)
        {
            var offset = localOffset;
            offset.x *= facingSign >= 0 ? 1f : -1f;
            return (Vector2)transform.position + offset;
        }

        public void ApplyTuning(float attackRange, int newTileDamage)
        {
            localOffset = new Vector2(Mathf.Abs(attackRange), localOffset.y);
            tileDamage = Mathf.Max(0, newTileDamage);
        }

        private void DamageTiles(Vector2 worldCenter)
        {
            if (tileDamage <= 0)
            {
                return;
            }

            var colliders = Physics2D.OverlapBoxAll(worldCenter, boxSize, 0f, tileMask);
            for (var i = 0; i < colliders.Length; i++)
            {
                var tile = colliders[i].GetComponentInParent<DestructibleTile>();
                if (tile == null || _tileHitBuffer.Contains(tile))
                {
                    continue;
                }

                _tileHitBuffer.Add(tile);
                tile.ApplyDamage(tileDamage);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(GetWorldCenter(1), boxSize);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(GetWorldCenter(-1), boxSize);
        }
    }
}
