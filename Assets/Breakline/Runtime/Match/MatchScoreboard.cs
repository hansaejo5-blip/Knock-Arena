using System;

namespace Breakline.Runtime.Match
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

        public int GetScore(int playerIndex)
        {
            Validate(playerIndex);
            return _scores[playerIndex];
        }

        public void AddPoint(int playerIndex)
        {
            Validate(playerIndex);
            _scores[playerIndex]++;
        }

        public int ResolveWinnerOrTie()
        {
            var winner = -1;
            var best = int.MinValue;
            var tie = false;

            for (var i = 0; i < _scores.Length; i++)
            {
                if (_scores[i] > best)
                {
                    best = _scores[i];
                    winner = i;
                    tie = false;
                }
                else if (_scores[i] == best)
                {
                    tie = true;
                }
            }

            return tie ? -1 : winner;
        }

        private void Validate(int playerIndex)
        {
            if (playerIndex < 0 || playerIndex >= _scores.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(playerIndex));
            }
        }
    }
}
