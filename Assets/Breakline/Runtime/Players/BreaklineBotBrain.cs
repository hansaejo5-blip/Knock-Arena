namespace Breakline.Runtime.Players
{
    public sealed class BreaklineBotBrain
    {
        public float GetMoveAxisPlaceholder(float selfX, float targetX)
        {
            if (targetX > selfX + 0.25f)
            {
                return 1f;
            }

            if (targetX < selfX - 0.25f)
            {
                return -1f;
            }

            return 0f;
        }
    }
}
