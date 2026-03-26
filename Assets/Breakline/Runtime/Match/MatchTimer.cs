using System;
using UnityEngine;

namespace Breakline.Runtime.Match
{
    public sealed class MatchTimer : MonoBehaviour
    {
        [SerializeField] private float durationSeconds = 120f;
        [SerializeField] private bool playOnStart = true;

        private float _remainingSeconds;

        public event Action<float> TimeChanged;
        public event Action TimerExpired;

        public float DurationSeconds => durationSeconds;
        public float RemainingSeconds => _remainingSeconds;
        public bool IsRunning { get; private set; }
        public bool IsExpired => _remainingSeconds <= 0f;

        private void Start()
        {
            ResetTimer();

            if (playOnStart)
            {
                StartTimer();
            }
        }

        private void Update()
        {
            if (!IsRunning || IsExpired)
            {
                return;
            }

            _remainingSeconds -= Time.deltaTime;
            if (_remainingSeconds < 0f)
            {
                _remainingSeconds = 0f;
            }

            TimeChanged?.Invoke(_remainingSeconds);

            if (_remainingSeconds <= 0f)
            {
                IsRunning = false;
                TimerExpired?.Invoke();
            }
        }

        public void ConfigureDuration(float newDurationSeconds)
        {
            durationSeconds = Mathf.Max(1f, newDurationSeconds);
            ResetTimer();
        }

        public void ResetTimer()
        {
            _remainingSeconds = Mathf.Max(1f, durationSeconds);
            IsRunning = false;
            TimeChanged?.Invoke(_remainingSeconds);
        }

        public void StartTimer()
        {
            if (IsExpired)
            {
                ResetTimer();
            }

            IsRunning = true;
            TimeChanged?.Invoke(_remainingSeconds);
        }

        public void StopTimer()
        {
            IsRunning = false;
        }
    }
}
