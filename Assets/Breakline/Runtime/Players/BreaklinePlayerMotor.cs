using Breakline.Runtime.Config;
using UnityEngine;

namespace Breakline.Runtime.Players
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class BreaklinePlayerMotor : MonoBehaviour
    {
        [SerializeField] private BreaklinePlayerSettings settings;
        [SerializeField] private LayerMask groundLayerMask = ~0;
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private BreaklinePlayerInput playerInput;
        [SerializeField] private BreaklineBotController botController;

        private Rigidbody2D _rigidbody2D;

        public bool IsGrounded { get; private set; }
        public int FacingDirection { get; private set; } = 1;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (settings == null)
            {
                return;
            }

            RefreshGrounded();

            var moveAxis = ResolveMoveAxis();
            if (Mathf.Abs(moveAxis) > 0.01f)
            {
                FacingDirection = moveAxis > 0f ? 1 : -1;
            }

            if (ResolveJumpPressed() && IsGrounded)
            {
                Jump();
            }
        }

        private void FixedUpdate()
        {
            if (settings == null)
            {
                return;
            }

            var targetX = Mathf.Clamp(ResolveMoveAxis() * settings.moveSpeed, -settings.maxHorizontalSpeed, settings.maxHorizontalSpeed);
            var velocity = _rigidbody2D.linearVelocity;
            velocity.x = targetX;
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

        private float ResolveMoveAxis()
        {
            if (botController != null && botController.enabled)
            {
                return botController.DesiredMoveAxis;
            }

            return playerInput != null ? playerInput.MoveAxis : 0f;
        }

        private bool ResolveJumpPressed()
        {
            return playerInput != null && playerInput.enabled && playerInput.JumpPressedThisFrame;
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
            var origin = groundCheckPoint != null ? groundCheckPoint.position : transform.position;
            var sample = (Vector2)origin + settings.groundCheckOffset;
            IsGrounded = Physics2D.OverlapCircle(sample, settings.groundCheckRadius, groundLayerMask);
        }
    }
}
