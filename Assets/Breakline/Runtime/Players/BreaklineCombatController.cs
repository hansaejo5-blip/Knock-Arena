using Breakline.Runtime.Config;
using Breakline.Runtime.Tiles;
using UnityEngine;

namespace Breakline.Runtime.Players
{
    [RequireComponent(typeof(BreaklinePlayerMotor))]
    public sealed class BreaklineCombatController : MonoBehaviour
    {
        [SerializeField] private BreaklinePlayerSettings settings;
        [SerializeField] private BreaklineMatchSettings matchSettings;
        [SerializeField] private Transform attackOrigin;
        [SerializeField] private BreaklinePlayerInput playerInput;
        [SerializeField] private BreaklineBotController botController;

        private BreaklinePlayerMotor _motor;
        private float _attackCooldown;

        private void Awake()
        {
            _motor = GetComponent<BreaklinePlayerMotor>();
        }

        private void Update()
        {
            if (settings == null || matchSettings == null || _motor == null)
            {
                return;
            }

            _attackCooldown -= Time.deltaTime;
            if (_attackCooldown > 0f || !ResolveAttackPressed())
            {
                return;
            }

            var origin = attackOrigin != null ? attackOrigin.position : transform.position;
            var sample = (Vector2)origin + Vector2.right * (_motor.FacingDirection * settings.attackRange);

            DamageTile(sample);
            PushOpponent(sample);
            _attackCooldown = settings.attackCooldownSeconds;
        }

        private bool ResolveAttackPressed()
        {
            if (botController != null && botController.enabled)
            {
                return botController.WantsAttackThisFrame;
            }

            return playerInput != null && playerInput.enabled && playerInput.AttackPressedThisFrame;
        }

        private void DamageTile(Vector2 sample)
        {
            var hits = Physics2D.OverlapCircleAll(sample, settings.attackRadius, matchSettings.floorLayerMask);
            for (var i = 0; i < hits.Length; i++)
            {
                var tile = hits[i].GetComponentInParent<BreaklineTile>();
                if (tile == null)
                {
                    continue;
                }

                tile.ApplyDamage(settings.tileDamagePerHit);
                return;
            }
        }

        private void PushOpponent(Vector2 sample)
        {
            var hits = Physics2D.OverlapCircleAll(sample, settings.attackRadius, matchSettings.playerLayerMask);
            for (var i = 0; i < hits.Length; i++)
            {
                var targetMotor = hits[i].GetComponentInParent<BreaklinePlayerMotor>();
                if (targetMotor == null || targetMotor == _motor)
                {
                    continue;
                }

                targetMotor.AddImpulse(new Vector2(_motor.FacingDirection * settings.pushForce, settings.pushUpForce));
                return;
            }
        }
    }
}
