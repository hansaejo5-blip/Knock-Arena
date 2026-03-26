using System;
using System.Collections.Generic;
using Breakline.Runtime.Config;
using Breakline.Runtime.Hud;
using Breakline.Runtime.Players;
using Breakline.Runtime.RingOut;
using UnityEngine;

namespace Breakline.Runtime.Match
{
    public sealed class BreaklineMatchController : MonoBehaviour
    {
        [SerializeField] private BreaklineMatchSettings settings;
        [SerializeField] private List<BreaklinePlayerAvatar> players = new();
        [SerializeField] private List<BreaklineSpawnPoint> spawnPoints = new();
        [SerializeField] private BreaklineHudBridge hudBridge;
        [SerializeField] private RingOutMonitor ringOutMonitor;
        [SerializeField] private LocalTrophyProgress localTrophyProgress;

        private readonly Dictionary<int, BreaklineSpawnPoint> _spawnLookup = new();
        private MatchClock _clock;
        private MatchScoreboard _scoreboard;
        private bool _isMatchComplete;

        public event Action<MatchResult, TrophyResult> MatchCompleted;

        private void Start()
        {
            if (settings == null)
            {
                Debug.LogError("BreaklineMatchController requires BreaklineMatchSettings.", this);
                enabled = false;
                return;
            }

            _clock = new MatchClock(settings.matchDurationSeconds);
            _scoreboard = new MatchScoreboard(players.Count);

            _spawnLookup.Clear();
            for (var i = 0; i < spawnPoints.Count; i++)
            {
                if (spawnPoints[i] != null)
                {
                    _spawnLookup[spawnPoints[i].PlayerIndex] = spawnPoints[i];
                }
            }

            if (ringOutMonitor != null)
            {
                ringOutMonitor.Initialize(settings.ringOutY);
                ringOutMonitor.PlayerRingOut += OnPlayerRingOut;
            }

            PublishHud();
        }

        private void OnDestroy()
        {
            if (ringOutMonitor != null)
            {
                ringOutMonitor.PlayerRingOut -= OnPlayerRingOut;
            }
        }

        private void Update()
        {
            if (_isMatchComplete || _clock == null)
            {
                return;
            }

            _clock.Tick(Time.deltaTime);
            PublishHud();

            if (_clock.IsExpired)
            {
                CompleteMatch();
            }
        }

        private void OnPlayerRingOut(BreaklinePlayerAvatar fallenPlayer)
        {
            if (_isMatchComplete || fallenPlayer == null || fallenPlayer.IsRespawning)
            {
                return;
            }

            var scoringPlayerIndex = ResolveOpponentIndex(fallenPlayer.PlayerIndex);
            if (scoringPlayerIndex >= 0)
            {
                _scoreboard.AddPoint(scoringPlayerIndex);
            }

            PublishHud();

            if (_spawnLookup.TryGetValue(fallenPlayer.PlayerIndex, out var spawnPoint))
            {
                StartCoroutine(fallenPlayer.RespawnRoutine(spawnPoint.transform.position, settings.respawnDelaySeconds));
            }
        }

        private int ResolveOpponentIndex(int fallenPlayerIndex)
        {
            for (var i = 0; i < players.Count; i++)
            {
                var player = players[i];
                if (player == null || player.PlayerIndex == fallenPlayerIndex)
                {
                    continue;
                }

                return player.PlayerIndex;
            }

            return -1;
        }

        private void CompleteMatch()
        {
            _isMatchComplete = true;

            for (var i = 0; i < players.Count; i++)
            {
                if (players[i] != null)
                {
                    players[i].SetGameplayEnabled(false);
                }
            }

            var result = MatchResultCalculator.Calculate(_scoreboard);
            var resultSummary = localTrophyProgress != null
                ? localTrophyProgress.ApplyMatchResult(result)
                : ResultSummaryGenerator.Create(result, 0);
            var trophyResult = new TrophyResult(
                resultSummary.Headline,
                resultSummary.TrophyDelta,
                resultSummary.NewTier.TierId);
            MatchCompleted?.Invoke(result, trophyResult);
            PublishHud(result, trophyResult);
        }

        private void PublishHud()
        {
            if (hudBridge == null || _scoreboard == null || _clock == null)
            {
                return;
            }

            hudBridge.Publish(new BreaklineHudSnapshot(
                _clock.RemainingSeconds,
                _scoreboard.GetScore(0),
                players.Count > 1 ? _scoreboard.GetScore(1) : 0,
                false,
                -1,
                string.Empty,
                0));
        }

        private void PublishHud(MatchResult result, TrophyResult trophyResult)
        {
            if (hudBridge == null || _scoreboard == null || _clock == null)
            {
                return;
            }

            hudBridge.Publish(new BreaklineHudSnapshot(
                _clock.RemainingSeconds,
                _scoreboard.GetScore(0),
                players.Count > 1 ? _scoreboard.GetScore(1) : 0,
                true,
                result.WinnerPlayerIndex,
                trophyResult.GradeId,
                trophyResult.TrophyDelta));
        }
    }
}
