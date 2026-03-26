using System;

namespace ArenaPrototype.Runtime.Match
{
    public sealed class MatchScoreboard
    {
        private readonly int[] _scores;

        public MatchScoreboard(int playerCount)
        {
            if (playerCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(playerCount));
            }

            _scores = new int[playerCount];
        }

        public int PlayerCount => _scores.Length;

        public int GetScore(int playerIndex)
        {
            ValidatePlayerIndex(playerIndex);
            return _scores[playerIndex];
        }

        public void AddPoint(int playerIndex)
        {
            ValidatePlayerIndex(playerIndex);
            _scores[playerIndex]++;
        }

        public int GetLeader()
        {
            var leader = -1;
            var topScore = int.MinValue;
            var tie = false;

            for (var i = 0; i < _scores.Length; i++)
            {
                if (_scores[i] > topScore)
                {
                    topScore = _scores[i];
                    leader = i;
                    tie = false;
                }
                else if (_scores[i] == topScore)
                {
                    tie = true;
                }
            }

            return tie ? -1 : leader;
        }

        private void ValidatePlayerIndex(int playerIndex)
        {
            if (playerIndex < 0 || playerIndex >= _scores.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(playerIndex));
            }
        }
    }
}
