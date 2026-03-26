using ArenaPrototype.Runtime.Config;
using ArenaPrototype.Runtime.Environment;
using UnityEngine;

namespace ArenaPrototype.Runtime.Players
{
    [RequireComponent(typeof(ArenaPlayerMotor))]
    public sealed class ArenaPlayerCombat : MonoBehaviour
    {
        [SerializeField] private PlayerCombatSettings settings;
        [SerializeField] private ArenaPrototypeSettings prototypeSettings;
        [SerializeField] private Transform actionOrigin;

        private ArenaPlayerInput _input;
        private ArenaPlayerMotor _motor;
        private float _shoveCooldown;
        private float _breakCooldown;

        private void Awake()
        {
            _input = GetComponent<ArenaPlayerInput>();
            _motor = GetComponent<ArenaPlayerMotor>();
        }

        private void Update()
        {
            if (_input == null || _motor == null || settings == null || prototypeSettings == null)
            {
                return;
            }

            _shoveCooldown -= Time.deltaTime;
            _breakCooldown -= Time.deltaTime;

            if (_input.ShovePressedThisFrame && _shoveCooldown <= 0f)
            {
                TryShove();
            }

            if (_input.BreakPressedThisFrame && _breakCooldown <= 0f)
            {
                TryBreakFloor();
            }
        }

        private void TryShove()
        {
            var origin = ResolveActionOrigin();
            var hit = Physics2D.OverlapCircle(
                origin + Vector2.right * (_motor.FacingDirection * settings.shoveRange),
                settings.breakProbeRadius,
                prototypeSettings.playerLayerMask);

            if (hit == null)
            {
                _shoveCooldown = settings.shoveCooldownSeconds * 0.35f;
                return;
            }

            var targetMotor = hit.GetComponentInParent<ArenaPlayerMotor>();
            if (targetMotor == null || targetMotor == _motor)
            {
                return;
            }

            var shoveDirection = new Vector2(_motor.FacingDirection, 0f).normalized;
            var impulse = shoveDirection * settings.shoveForce + Vector2.up * settings.shoveUpForce;
            targetMotor.AddImpulse(impulse);
            _shoveCooldown = settings.shoveCooldownSeconds;
        }

        private void TryBreakFloor()
        {
            var origin = ResolveActionOrigin();
            var samplePoints = new[]
            {
                origin + Vector2.right * (_motor.FacingDirection * settings.breakRange),
                origin + new Vector2(_motor.FacingDirection * settings.breakRange * 0.8f, -0.35f),
                origin + Vector2.down * 0.6f
            };

            for (var i = 0; i < samplePoints.Length; i++)
            {
                var hits = Physics2D.OverlapCircleAll(
                    samplePoints[i],
                    settings.breakProbeRadius,
                    prototypeSettings.floorLayerMask);

                for (var j = 0; j < hits.Length; j++)
                {
                    var tile = hits[j].GetComponentInParent<FloorTile>();
                    if (tile == null || tile.IsBroken)
                    {
                        continue;
                    }

                    tile.Break();
                    _breakCooldown = settings.breakCooldownSeconds;
                    return;
                }
            }

            _breakCooldown = settings.breakCooldownSeconds * 0.35f;
        }

        private Vector2 ResolveActionOrigin()
        {
            return actionOrigin != null ? actionOrigin.position : transform.position;
        }

        private void OnDrawGizmosSelected()
        {
            if (settings == null)
            {
                return;
            }

            var facing = Application.isPlaying && _motor != null ? _motor.FacingDirection : 1;
            var origin = ResolveActionOrigin();

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(origin + Vector2.right * (facing * settings.shoveRange), settings.breakProbeRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(origin + Vector2.right * (facing * settings.breakRange), settings.breakProbeRadius);
            Gizmos.DrawWireSphere(origin + new Vector2(facing * settings.breakRange * 0.8f, -0.35f), settings.breakProbeRadius);
            Gizmos.DrawWireSphere(origin + Vector2.down * 0.6f, settings.breakProbeRadius);
        }
    }
}
