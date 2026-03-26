using UnityEngine;

namespace Breakline.Runtime.Players
{
    public sealed class BreaklineBotController : MonoBehaviour
    {
        [SerializeField] private BreaklinePlayerAvatar self;
        [SerializeField] private BreaklinePlayerAvatar target;

        private readonly BreaklineBotBrain _brain = new();

        public float DesiredMoveAxis { get; private set; }
        public bool WantsAttackThisFrame { get; private set; }

        private void Update()
        {
            WantsAttackThisFrame = false;
            if (self == null || target == null)
            {
                DesiredMoveAxis = 0f;
                return;
            }

            DesiredMoveAxis = _brain.GetMoveAxisPlaceholder(self.transform.position.x, target.transform.position.x);
            WantsAttackThisFrame = Mathf.Abs(target.transform.position.x - self.transform.position.x) < 1f;
        }
    }
}
