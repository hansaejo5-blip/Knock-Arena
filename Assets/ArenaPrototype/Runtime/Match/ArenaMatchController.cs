using System.Collections;
using System.Collections.Generic;
using ArenaPrototype.Runtime.Config;
using ArenaPrototype.Runtime.Players;
using UnityEngine;

namespace ArenaPrototype.Runtime.Match
{
    public sealed class ArenaMatchController : MonoBehaviour
    {
        [SerializeField] private ArenaPrototypeSettings settings;
        [SerializeField] private List<ArenaPlayerAvatar> players = new();
        [SerializeField] private List<ArenaPlayerSpawnPoint> spawnPoints = new();

        private readonly Dictionary<int, ArenaPlayerSpawnPoint> _spawnLookup = new();
        private MatchClock _clock;
        private MatchScoreboard _scoreboard;
        private bool _matchEnded;

        public float RemainingSeconds => _clock?.RemainingSeconds ?? 0f;
        public bool MatchEnded => _matchEnded;

        private void Start()
        {
            if (settings == null)
            {
                Debug.LogError("ArenaMatchController requires ArenaPrototypeSettings.", this);
                enabled = false;
                return;
            }

            _clock = new MatchClock(settings.matchDurationSeconds);
            _scoreboard = new MatchScoreboard(players.Count);

            _spawnLookup.Clear();
            for (var i = 0; i < spawnPoints.Count; i++)
            {
                _spawnLookup[spawnPoints[i].PlayerIndex] = spawnPoints[i];
            }
        }

        private void Update()
        {
            if (_matchEnded || _clock == null)
            {
                return;
            }

            _clock.Advance(Time.deltaTime);

            for (var i = 0; i < players.Count; i++)
            {
                var player = players[i];
                if (player == null || player.IsRespawning)
                {
                    continue;
                }

                if (player.transform.position.y < settings.ringOutY)
                {
                    HandleRingOut(player);
                }
            }

            if (_clock.IsExpired)
            {
                EndMatch();
            }
        }

        public int GetScore(int playerIndex)
        {
            return _scoreboard.GetScore(playerIndex);
        }

        private void HandleRingOut(ArenaPlayerAvatar fallenPlayer)
        {
            var scoringPlayer = ResolveOpponentIndex(fallenPlayer.PlayerIndex);
            if (scoringPlayer >= 0)
            {
                _scoreboard.AddPoint(scoringPlayer);
            }

            if (_spawnLookup.TryGetValue(fallenPlayer.PlayerIndex, out var spawnPoint))
            {
                StartCoroutine(fallenPlayer.RespawnRoutine(
                    spawnPoint.transform.position,
                    settings.respawnDelaySeconds));
            }
        }

        private int ResolveOpponentIndex(int fallenPlayerIndex)
        {
            for (var i = 0; i < players.Count; i++)
            {
                if (players[i] == null)
                {
                    continue;
                }

                if (players[i].PlayerIndex != fallenPlayerIndex)
                {
                    return players[i].PlayerIndex;
                }
            }

            return -1;
        }

        private void EndMatch()
        {
            _matchEnded = true;

            for (var i = 0; i < players.Count; i++)
            {
                if (players[i] != null)
                {
                    players[i].SetGameplayEnabled(false);
                }
            }

            var leader = _scoreboard.GetLeader();
            var resultText = leader >= 0
                ? $"Player {leader + 1} wins with {_scoreboard.GetScore(leader)} points."
                : "Match ended in a tie.";

            Debug.Log($"Arena Match Complete. {resultText}", this);
        }
    }
}
