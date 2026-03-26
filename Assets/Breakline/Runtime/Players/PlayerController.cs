using System;
using Breakline.Runtime.Combat;
using Breakline.Runtime.Characters;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Breakline.Runtime.Players
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private int playerIndex;

        [Header("Prototype Input")]
        [SerializeField] private bool readKeyboardInput = true;
        [SerializeField] private Key moveLeftKey = Key.A;
        [SerializeField] private Key moveRightKey = Key.D;
        [SerializeField] private Key dashKey = Key.LeftShift;
        [SerializeField] private Key attackKey = Key.Space;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 8.5f;
        [SerializeField] private float groundAcceleration = 80f;
        [SerializeField] private float groundDeceleration = 90f;
        [SerializeField] private float maxHorizontalSpeed = 8.5f;

        [Header("Dash")]
        [SerializeField] private float dashSpeed = 16f;
        [SerializeField] private float dashDurationSeconds = 0.12f;
        [SerializeField] private float dashCooldownSeconds = 0.75f;

        [Header("Combat")]
        [SerializeField] private AttackHitbox attackHitbox;
        [SerializeField] private float attackCooldownSeconds = 0.25f;
        [SerializeField] private float attackHorizontalKnockback = 9f;
        [SerializeField] private float attackVerticalKnockback = 2f;
        [SerializeField] private float hitStunDurationSeconds = 0.12f;

        [Header("Hit Reaction")]
        [SerializeField] private float hitStopHorizontalDamping = 0.2f;

        private readonly CooldownTimer _attackCooldown = new();
        private readonly CooldownTimer _dashCooldown = new();

        private Rigidbody2D _body;
        private float _moveInput;
        private bool _attackRequested;
        private bool _dashRequested;
        private float _stunRemainingSeconds;
        private float _dashRemainingSeconds;
        private int _facingSign = 1;
        private int _dashDirectionSign = 1;
        private bool _gameplayEnabled = true;
        private float _lastPublishedDashNormalized = -1f;
        private float _lastPublishedAttackNormalized = -1f;

        public event Action<PlayerCombatHudSnapshot> CombatHudChanged;

        public int PlayerIndex => playerIndex;
        public int FacingSign => _facingSign;
        public FacingDirection FacingDirection => _facingSign >= 0 ? FacingDirection.Right : FacingDirection.Left;
        public bool IsDashing => _dashRemainingSeconds > 0f;
        public bool IsStunned => _stunRemainingSeconds > 0f;
        public bool CanAttack => _attackCooldown.IsReady && !IsStunned;
        public bool CanDash => _dashCooldown.IsReady && !IsDashing && !IsStunned;

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!_gameplayEnabled)
            {
                _moveInput = 0f;
                _attackRequested = false;
                _dashRequested = false;
                PublishCombatHud(true);
                return;
            }

            PollPrototypeInput();

            _attackCooldown.Tick(Time.deltaTime);
            _dashCooldown.Tick(Time.deltaTime);

            if (_stunRemainingSeconds > 0f)
            {
                _stunRemainingSeconds -= Time.deltaTime;
                if (_stunRemainingSeconds < 0f)
                {
                    _stunRemainingSeconds = 0f;
                }
            }

            if (_dashRemainingSeconds > 0f)
            {
                _dashRemainingSeconds -= Time.deltaTime;
                if (_dashRemainingSeconds < 0f)
                {
                    _dashRemainingSeconds = 0f;
                }
            }

            UpdateFacingDirection();

            if (_dashRequested && CanDash)
            {
                StartDash();
            }

            if (_attackRequested && CanAttack)
            {
                PerformAttack();
            }

            _attackRequested = false;
            _dashRequested = false;
            PublishCombatHud();
        }

        private void FixedUpdate()
        {
            if (!_gameplayEnabled)
            {
                _body.linearVelocity = new Vector2(0f, _body.linearVelocity.y);
                return;
            }

            if (IsDashing)
            {
                _body.linearVelocity = new Vector2(_dashDirectionSign * dashSpeed, _body.linearVelocity.y);
                return;
            }

            if (IsStunned)
            {
                var stunnedVelocity = _body.linearVelocity;
                stunnedVelocity.x = Mathf.MoveTowards(stunnedVelocity.x, 0f, groundDeceleration * hitStopHorizontalDamping * Time.fixedDeltaTime);
                _body.linearVelocity = stunnedVelocity;
                return;
            }

            var targetSpeed = _moveInput * moveSpeed;
            var velocity = _body.linearVelocity;
            var acceleration = Mathf.Abs(targetSpeed) > 0.01f ? groundAcceleration : groundDeceleration;
            velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
            velocity.x = Mathf.Clamp(velocity.x, -maxHorizontalSpeed, maxHorizontalSpeed);
            _body.linearVelocity = velocity;
        }

        // Mobile controls can call this directly instead of using keyboard input.
        public void SetMoveInput(float inputX)
        {
            _moveInput = Mathf.Clamp(inputX, -1f, 1f);
        }

        public void RequestDash()
        {
            _dashRequested = true;
        }

        public void RequestAttack()
        {
            _attackRequested = true;
        }

        public void SetGameplayEnabled(bool enabled)
        {
            _gameplayEnabled = enabled;
            if (!enabled)
            {
                _moveInput = 0f;
                _attackRequested = false;
                _dashRequested = false;
                _dashRemainingSeconds = 0f;
                _stunRemainingSeconds = 0f;
                _body.linearVelocity = new Vector2(0f, _body.linearVelocity.y);
            }

            PublishCombatHud(true);
        }

        // CharacterLoadout can call this at runtime to swap archetype data without changing shared controller code.
        public void ApplyCharacterTuning(CharacterTuningData tuning)
        {
            moveSpeed = Mathf.Max(0f, tuning.movementSpeed);
            maxHorizontalSpeed = Mathf.Max(0f, tuning.movementSpeed);
            dashCooldownSeconds = Mathf.Max(0f, tuning.dashCooldownSeconds);
            attackHorizontalKnockback = Mathf.Max(0f, tuning.attackForce);

            if (attackHitbox != null)
            {
                attackHitbox.ApplyTuning(tuning.attackRange, tuning.tileDamage);
            }
        }

        public void ReceiveHit(PlayerController attacker, CombatHitData hitData, Vector2 hitPoint)
        {
            _stunRemainingSeconds = Mathf.Max(_stunRemainingSeconds, hitData.StunDurationSeconds);
            _dashRemainingSeconds = 0f;

            var velocity = _body.linearVelocity;
            velocity.x *= hitStopHorizontalDamping;
            _body.linearVelocity = velocity;

            var impulse = KnockbackResolver.ResolveImpulse(hitData, hitPoint);
            _body.AddForce(impulse, ForceMode2D.Impulse);
        }

        private void PollPrototypeInput()
        {
            if (!readKeyboardInput)
            {
                return;
            }

            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                _moveInput = 0f;
                return;
            }

            var left = keyboard[moveLeftKey].isPressed ? -1f : 0f;
            var right = keyboard[moveRightKey].isPressed ? 1f : 0f;
            _moveInput = left + right;

            if (keyboard[dashKey].wasPressedThisFrame)
            {
                _dashRequested = true;
            }

            if (keyboard[attackKey].wasPressedThisFrame)
            {
                _attackRequested = true;
            }
        }

        private void UpdateFacingDirection()
        {
            if (Mathf.Abs(_moveInput) > 0.01f)
            {
                _facingSign = _moveInput > 0f ? 1 : -1;
            }
        }

        private void StartDash()
        {
            _dashDirectionSign = Mathf.Abs(_moveInput) > 0.01f ? (_moveInput > 0f ? 1 : -1) : _facingSign;
            _dashRemainingSeconds = dashDurationSeconds;
            _dashCooldown.Start(dashCooldownSeconds);
            PublishCombatHud(true);
        }

        private void PerformAttack()
        {
            if (attackHitbox == null)
            {
                return;
            }

            var hitData = new CombatHitData(
                attackHitbox.GetWorldCenter(_facingSign),
                _facingSign,
                attackHorizontalKnockback,
                attackVerticalKnockback,
                hitStunDurationSeconds);

            attackHitbox.PerformHit(this, _facingSign, hitData);
            _attackCooldown.Start(attackCooldownSeconds);
            PublishCombatHud(true);
        }

        private void PublishCombatHud(bool force = false)
        {
            var dashNormalized = dashCooldownSeconds > 0f ? Mathf.Clamp01(_dashCooldown.RemainingSeconds / dashCooldownSeconds) : 0f;
            var attackNormalized = attackCooldownSeconds > 0f ? Mathf.Clamp01(_attackCooldown.RemainingSeconds / attackCooldownSeconds) : 0f;

            if (!force &&
                Mathf.Approximately(dashNormalized, _lastPublishedDashNormalized) &&
                Mathf.Approximately(attackNormalized, _lastPublishedAttackNormalized))
            {
                return;
            }

            _lastPublishedDashNormalized = dashNormalized;
            _lastPublishedAttackNormalized = attackNormalized;

            CombatHudChanged?.Invoke(new PlayerCombatHudSnapshot(
                CanDash,
                CanAttack,
                dashNormalized,
                attackNormalized));
        }
    }
}
