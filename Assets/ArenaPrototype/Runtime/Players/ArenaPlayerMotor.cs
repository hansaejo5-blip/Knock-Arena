using ArenaPrototype.Runtime.Config;
using UnityEngine;

namespace ArenaPrototype.Runtime.Players
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class ArenaPlayerMotor : MonoBehaviour
    {
        [SerializeField] private PlayerCombatSettings settings;
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private LayerMask groundLayerMask = ~0;

        private Rigidbody2D _rigidbody2D;
        private ArenaPlayerInput _input;
        private int _remainingAirJumps;

        public int FacingDirection { get; private set; } = 1;
        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _input = GetComponent<ArenaPlayerInput>();
            _remainingAirJumps = settings != null ? settings.extraAirJumps : 0;
        }

        private void Update()
        {
            if (settings == null || _input == null)
            {
                return;
            }

            RefreshGrounded();

            if (Mathf.Abs(_input.MoveAxis) > 0.01f)
            {
                FacingDirection = _input.MoveAxis > 0f ? 1 : -1;
            }

            if (_input.JumpPressedThisFrame)
            {
                TryJump();
            }
        }

        private void FixedUpdate()
        {
            if (settings == null || _input == null)
            {
                return;
            }

            var velocity = _rigidbody2D.linearVelocity;
            velocity.x = _input.MoveAxis * settings.moveSpeed;
            _rigidbody2D.linearVelocity = velocity;
        }

        public void AddImpulse(Vector2 impulse)
        {
            _rigidbody2D.AddForce(impulse, ForceMode2D.Impulse);
        }

        public void ResetMotion()
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0f;
        }

        private void TryJump()
        {
            if (IsGrounded)
            {
                Jump();
                _remainingAirJumps = settings.extraAirJumps;
                return;
            }

            if (_remainingAirJumps <= 0)
            {
                return;
            }

            _remainingAirJumps--;
            Jump();
        }

        private void Jump()
        {
            var velocity = _rigidbody2D.linearVelocity;
            velocity.y = 0f;
            _rigidbody2D.linearVelocity = velocity;
            _rigidbody2D.AddForce(Vector2.up * settings.jumpImpulse, ForceMode2D.Impulse);
        }

        private void RefreshGrounded()
        {
            var checkPosition = ResolveGroundCheckPosition();
            IsGrounded = Physics2D.OverlapCircle(
                checkPosition,
                settings.groundCheckRadius,
                groundLayerMask);

            if (IsGrounded)
            {
                _remainingAirJumps = settings.extraAirJumps;
            }
        }

        private Vector2 ResolveGroundCheckPosition()
        {
            if (groundCheckPoint != null)
            {
                return groundCheckPoint.position;
            }

            var anchor = transform.position;
            if (settings.groundCheck == TransformAnchor.Feet)
            {
                anchor += Vector3.down * 0.5f;
            }

            return (Vector2)anchor + settings.groundCheckOffset;
        }

        private void OnDrawGizmosSelected()
        {
            if (settings == null)
            {
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(ResolveGroundCheckPosition(), settings.groundCheckRadius);
        }
    }
}
