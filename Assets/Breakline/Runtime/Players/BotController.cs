using Breakline.Runtime.Combat;
using Breakline.Runtime.Tiles;
using UnityEngine;

namespace Breakline.Runtime.Players
{
    // Attach this to the same GameObject as PlayerController to drive a bot player.
    // For a bot-controlled player, disable PlayerController.readKeyboardInput in the Inspector.
    public sealed class BotController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController self;
        [SerializeField] private PlayerController target;
        [SerializeField] private Rigidbody2D selfBody;

        [Header("Arena Read")]
        [SerializeField] private LayerMask tileMask = ~0;
        [SerializeField] private float arenaMinX = -6f;
        [SerializeField] private float arenaMaxX = 6f;
        [SerializeField] private float edgeDangerDistance = 1.25f;
        [SerializeField] private float tileProbeDistance = 1f;
        [SerializeField] private float tileProbeDepth = 2f;

        [Header("Spacing")]
        [SerializeField] private float desiredFightDistance = 1.1f;
        [SerializeField] private float pressureDistance = 1.6f;
        [SerializeField] private float retreatDistance = 0.55f;
        [SerializeField] private float engageDistance = 4f;

        [Header("Attack")]
        [SerializeField] private float attackRange = 1.15f;
        [SerializeField] private float attackCommitCooldownSeconds = 0.22f;

        [Header("Dash")]
        [SerializeField] private float dashDecisionIntervalSeconds = 0.45f;
        [SerializeField] private float dashUseChance = 0.35f;
        [SerializeField] private float dashEngageDistanceMin = 1.6f;
        [SerializeField] private float dashEngageDistanceMax = 3.6f;
        [SerializeField] private float dashEscapeUnsafeBias = 0.8f;

        [Header("Debug")]
        [SerializeField] private bool drawDebug;

        private readonly CooldownTimer _attackCommitCooldown = new();
        private readonly CooldownTimer _dashDecisionCooldown = new();

        public BotDecisionState CurrentState { get; private set; }
        public float CurrentMoveInput { get; private set; }
        public bool CurrentTileUnsafe { get; private set; }
        public bool ForwardTileUnsafe { get; private set; }
        public bool TargetTileUnsafe { get; private set; }

        private void Reset()
        {
            self = GetComponent<PlayerController>();
            selfBody = GetComponent<Rigidbody2D>();
        }

        private void Awake()
        {
            if (self == null)
            {
                self = GetComponent<PlayerController>();
            }

            if (selfBody == null)
            {
                selfBody = GetComponent<Rigidbody2D>();
            }
        }

        private void Update()
        {
            if (self == null || target == null || !isActiveAndEnabled)
            {
                return;
            }

            _attackCommitCooldown.Tick(Time.deltaTime);
            _dashDecisionCooldown.Tick(Time.deltaTime);

            var selfPosition = (Vector2)self.transform.position;
            var targetPosition = (Vector2)target.transform.position;
            var toTarget = targetPosition - selfPosition;
            var horizontalDistance = Mathf.Abs(toTarget.x);
            var targetDirection = toTarget.x >= 0f ? 1f : -1f;

            CurrentTileUnsafe = IsUnsafeTile(selfPosition);
            ForwardTileUnsafe = IsUnsafeTile(selfPosition + Vector2.right * (self.FacingSign * tileProbeDistance));
            TargetTileUnsafe = IsUnsafeTile(targetPosition);

            CurrentState = DecideState(horizontalDistance, targetDirection, selfPosition, targetPosition);
            CurrentMoveInput = ResolveMoveInput(CurrentState, horizontalDistance, targetDirection, selfPosition, targetPosition);

            self.SetMoveInput(CurrentMoveInput);

            if (ShouldAttack(horizontalDistance))
            {
                self.RequestAttack();
                _attackCommitCooldown.Start(attackCommitCooldownSeconds);
            }

            if (ShouldDash(horizontalDistance, selfPosition, targetPosition))
            {
                self.RequestDash();
                _dashDecisionCooldown.Start(dashDecisionIntervalSeconds);
            }
        }

        private BotDecisionState DecideState(float horizontalDistance, float targetDirection, Vector2 selfPosition, Vector2 targetPosition)
        {
            if (CurrentTileUnsafe)
            {
                return BotDecisionState.Recover;
            }

            if (horizontalDistance <= retreatDistance && !ShouldPressureTowardDanger(targetPosition))
            {
                return BotDecisionState.Retreat;
            }

            if (ShouldPressureTowardDanger(targetPosition) || (horizontalDistance <= pressureDistance && !ForwardTileUnsafe))
            {
                return BotDecisionState.Pressure;
            }

            if (horizontalDistance <= engageDistance)
            {
                return BotDecisionState.Advance;
            }

            return BotDecisionState.Idle;
        }

        private float ResolveMoveInput(BotDecisionState state, float horizontalDistance, float targetDirection, Vector2 selfPosition, Vector2 targetPosition)
        {
            switch (state)
            {
                case BotDecisionState.Recover:
                    return ResolveRecoveryDirection(selfPosition);

                case BotDecisionState.Retreat:
                    return ForwardTileUnsafe ? 0f : -targetDirection;

                case BotDecisionState.Pressure:
                    if (horizontalDistance < desiredFightDistance * 0.8f)
                    {
                        return 0f;
                    }

                    return ForwardTileUnsafe ? 0f : targetDirection;

                case BotDecisionState.Advance:
                    if (horizontalDistance <= desiredFightDistance)
                    {
                        return 0f;
                    }

                    return ForwardTileUnsafe ? 0f : targetDirection;

                default:
                    return 0f;
            }
        }

        private bool ShouldAttack(float horizontalDistance)
        {
            if (_attackCommitCooldown.IsReady == false || !self.CanAttack)
            {
                return false;
            }

            if (horizontalDistance > attackRange)
            {
                return false;
            }

            return CurrentState == BotDecisionState.Pressure || CurrentState == BotDecisionState.Advance;
        }

        private bool ShouldDash(float horizontalDistance, Vector2 selfPosition, Vector2 targetPosition)
        {
            if (!_dashDecisionCooldown.IsReady || !self.CanDash)
            {
                return false;
            }

            if (CurrentTileUnsafe)
            {
                return Random.value < dashEscapeUnsafeBias;
            }

            if (horizontalDistance < dashEngageDistanceMin || horizontalDistance > dashEngageDistanceMax)
            {
                return false;
            }

            if (ForwardTileUnsafe)
            {
                return false;
            }

            if (ShouldPressureTowardDanger(targetPosition))
            {
                return Random.value < Mathf.Clamp01(dashUseChance + 0.15f);
            }

            return Random.value < dashUseChance;
        }

        private bool ShouldPressureTowardDanger(Vector2 targetPosition)
        {
            return IsNearArenaEdge(targetPosition.x) || TargetTileUnsafe;
        }

        private bool IsNearArenaEdge(float positionX)
        {
            return positionX <= arenaMinX + edgeDangerDistance || positionX >= arenaMaxX - edgeDangerDistance;
        }

        private float ResolveRecoveryDirection(Vector2 selfPosition)
        {
            var leftSample = EvaluateSafetyScore(selfPosition + Vector2.left * tileProbeDistance);
            var rightSample = EvaluateSafetyScore(selfPosition + Vector2.right * tileProbeDistance);

            if (leftSample > rightSample)
            {
                return -1f;
            }

            if (rightSample > leftSample)
            {
                return 1f;
            }

            var centerX = (arenaMinX + arenaMaxX) * 0.5f;
            return selfPosition.x >= centerX ? -1f : 1f;
        }

        private float EvaluateSafetyScore(Vector2 samplePosition)
        {
            var score = 0f;
            if (!IsUnsafeTile(samplePosition))
            {
                score += 1f;
            }

            var distanceToLeft = Mathf.Abs(samplePosition.x - arenaMinX);
            var distanceToRight = Mathf.Abs(arenaMaxX - samplePosition.x);
            score += Mathf.Min(distanceToLeft, distanceToRight) * 0.2f;
            return score;
        }

        private bool IsUnsafeTile(Vector2 samplePosition)
        {
            var hit = Physics2D.Raycast(samplePosition, Vector2.down, tileProbeDepth, tileMask);
            if (hit.collider == null)
            {
                return true;
            }

            var tile = hit.collider.GetComponentInParent<DestructibleTile>();
            if (tile == null)
            {
                return false;
            }

            return tile.IsBroken;
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawDebug)
            {
                return;
            }

            var origin = (Vector2)transform.position;
            DrawProbe(origin, CurrentTileUnsafe ? Color.red : Color.green);
            DrawProbe(origin + Vector2.right * tileProbeDistance, ForwardTileUnsafe ? Color.red : Color.green);
            DrawProbe(origin + Vector2.left * tileProbeDistance, Color.yellow);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(new Vector3(arenaMinX, transform.position.y - 3f, 0f), new Vector3(arenaMinX, transform.position.y + 3f, 0f));
            Gizmos.DrawLine(new Vector3(arenaMaxX, transform.position.y - 3f, 0f), new Vector3(arenaMaxX, transform.position.y + 3f, 0f));
        }

        private void DrawProbe(Vector2 origin, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(origin, origin + Vector2.down * tileProbeDepth);
        }
    }
}
