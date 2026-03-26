namespace Breakline.Runtime.Match
{
    public sealed class MatchClock
    {
        public MatchClock(float durationSeconds)
        {
            DurationSeconds = durationSeconds < 0f ? 0f : durationSeconds;
            RemainingSeconds = DurationSeconds;
        }

        public float DurationSeconds { get; }
        public float RemainingSeconds { get; private set; }
        public bool IsExpired => RemainingSeconds <= 0f;

        public void Tick(float deltaTime)
        {
            if (IsExpired || deltaTime <= 0f)
            {
                return;
            }

            RemainingSeconds -= deltaTime;
            if (RemainingSeconds < 0f)
            {
                RemainingSeconds = 0f;
            }
        }
    }
}
