using UnityEngine;
using UnityEngine.InputSystem;

namespace Breakline.Runtime.Players
{
    public sealed class BreaklinePlayerInput : MonoBehaviour
    {
        [SerializeField] private Key moveLeftKey = Key.A;
        [SerializeField] private Key moveRightKey = Key.D;
        [SerializeField] private Key jumpKey = Key.W;
        [SerializeField] private Key attackKey = Key.Space;

        public float MoveAxis { get; private set; }
        public bool JumpPressedThisFrame { get; private set; }
        public bool AttackPressedThisFrame { get; private set; }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                MoveAxis = 0f;
                JumpPressedThisFrame = false;
                AttackPressedThisFrame = false;
                return;
            }

            var left = keyboard[moveLeftKey].isPressed ? -1f : 0f;
            var right = keyboard[moveRightKey].isPressed ? 1f : 0f;
            MoveAxis = left + right;
            JumpPressedThisFrame = keyboard[jumpKey].wasPressedThisFrame;
            AttackPressedThisFrame = keyboard[attackKey].wasPressedThisFrame;
        }
    }
}
