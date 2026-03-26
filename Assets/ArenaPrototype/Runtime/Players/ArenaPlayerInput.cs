using UnityEngine;
using UnityEngine.InputSystem;

namespace ArenaPrototype.Runtime.Players
{
    public sealed class ArenaPlayerInput : MonoBehaviour
    {
        [Header("Horizontal")]
        [SerializeField] private Key moveLeftKey = Key.A;
        [SerializeField] private Key moveRightKey = Key.D;

        [Header("Actions")]
        [SerializeField] private Key jumpKey = Key.W;
        [SerializeField] private Key shoveKey = Key.Space;
        [SerializeField] private Key breakKey = Key.LeftShift;

        public float MoveAxis { get; private set; }
        public bool JumpPressedThisFrame { get; private set; }
        public bool ShovePressedThisFrame { get; private set; }
        public bool BreakPressedThisFrame { get; private set; }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                MoveAxis = 0f;
                JumpPressedThisFrame = false;
                ShovePressedThisFrame = false;
                BreakPressedThisFrame = false;
                return;
            }

            var left = keyboard[moveLeftKey].isPressed ? -1f : 0f;
            var right = keyboard[moveRightKey].isPressed ? 1f : 0f;

            MoveAxis = left + right;
            JumpPressedThisFrame = keyboard[jumpKey].wasPressedThisFrame;
            ShovePressedThisFrame = keyboard[shoveKey].wasPressedThisFrame;
            BreakPressedThisFrame = keyboard[breakKey].wasPressedThisFrame;
        }
    }
}
