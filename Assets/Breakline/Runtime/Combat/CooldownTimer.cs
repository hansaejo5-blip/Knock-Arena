namespace Breakline.Runtime.Combat
{
    public struct CooldownTimer
    {
        private float _remainingSeconds;

        public bool IsReady => _remainingSeconds <= 0f;
        public float RemainingSeconds => _remainingSeconds;

        public void Start(float durationSeconds)
        {
            _remainingSeconds = durationSeconds > 0f ? durationSeconds : 0f;
        }

        public void Tick(float deltaTime)
        {
            if (_remainingSeconds <= 0f || deltaTime <= 0f)
            {
                return;
            }

            _remainingSeconds -= deltaTime;
            if (_remainingSeconds < 0f)
            {
                _remainingSeconds = 0f;
            }
        }
    }
}
