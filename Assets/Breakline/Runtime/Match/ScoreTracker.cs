using System;
using UnityEngine;

namespace Breakline.Runtime.Match
{
    public sealed class ScoreTracker : MonoBehaviour
    {
        [SerializeField] private int playerCount = 2;
        [SerializeField] private bool enableCenterControlScoring;
        [SerializeField] private int centerControlScorePerTick = 1;

        private int[] _scores;

        public event Action<int, int> ScoreChanged;
        public event Action ScoresReset;

        public bool EnableCenterControlScoring => enableCenterControlScoring;
        public int CenterControlScorePerTick => centerControlScorePerTick;
        public int PlayerCount => _scores != null ? _scores.Length : Mathf.Max(1, playerCount);

        private void Awake()
        {
            Configure(playerCount);
        }

        public void Configure(int totalPlayers)
        {
            playerCount = Mathf.Max(1, totalPlayers);
            _scores = new int[playerCount];
            ScoresReset?.Invoke();
        }

        public int GetScore(int playerIndex)
        {
            ValidateIndex(playerIndex);
            return _scores[playerIndex];
        }

        public void AddRingOutPoint(int playerIndex, int points = 1)
        {
            if (points <= 0)
            {
                return;
            }

            ValidateIndex(playerIndex);
            _scores[playerIndex] += points;
            ScoreChanged?.Invoke(playerIndex, _scores[playerIndex]);
        }

        public void AddCenterControlPoint(int playerIndex)
        {
            if (!enableCenterControlScoring)
            {
                return;
            }

            AddRingOutPoint(playerIndex, centerControlScorePerTick);
        }

        public void ResetScores()
        {
            for (var i = 0; i < _scores.Length; i++)
            {
                _scores[i] = 0;
            }

            ScoresReset?.Invoke();
        }

        private void ValidateIndex(int playerIndex)
        {
            if (playerIndex < 0 || playerIndex >= _scores.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(playerIndex));
            }
        }
    }
}
