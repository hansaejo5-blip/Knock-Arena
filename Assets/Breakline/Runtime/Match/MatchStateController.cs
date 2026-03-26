using System;
using System.Collections;
using System.Collections.Generic;
using Breakline.Runtime.Hud;
using Breakline.Runtime.Players;
using UnityEngine;

namespace Breakline.Runtime.Match
{
    public sealed class MatchStateController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MatchTimer matchTimer;
        [SerializeField] private ScoreTracker scoreTracker;
        [SerializeField] private RingOutDetector ringOutDetector;
        [SerializeField] private BreaklineHudBridge hudBridge;
        [SerializeField] private LocalTrophyProgress localTrophyProgress;

        [Header("Players")]
        [SerializeField] private List<PlayerController> players = new();
        [SerializeField] private List<Transform> spawnPoints = new();

        [Header("Respawn")]
        [SerializeField] private float respawnDelaySeconds = 1.1f;

        public event Action<MatchPhase> PhaseChanged;
        public event Action<MatchResult> MatchEnded;
        public event Action<ResultSummary> ResultSummaryReady;
        public event Action<PlayerController, int> RingOutScored;
        public event Action<string, MatchResult> TrophyIntegrationRequested;

        public MatchPhase Phase { get; private set; } = MatchPhase.WaitingToStart;

        private void OnEnable()
        {
            if (matchTimer != null)
            {
                matchTimer.TimeChanged += OnTimeChanged;
                matchTimer.TimerExpired += OnTimerExpired;
            }

            if (scoreTracker != null)
            {
                scoreTracker.ScoreChanged += OnScoreChanged;
                scoreTracker.ScoresReset += OnScoresReset;
            }

            if (ringOutDetector != null)
            {
                ringOutDetector.PlayerRingOut += OnPlayerRingOut;
            }
        }

        private void OnDisable()
        {
            if (matchTimer != null)
            {
                matchTimer.TimeChanged -= OnTimeChanged;
                matchTimer.TimerExpired -= OnTimerExpired;
            }

            if (scoreTracker != null)
            {
                scoreTracker.ScoreChanged -= OnScoreChanged;
                scoreTracker.ScoresReset -= OnScoresReset;
            }

            if (ringOutDetector != null)
            {
                ringOutDetector.PlayerRingOut -= OnPlayerRingOut;
            }
        }

        private void Start()
        {
            if (scoreTracker != null)
            {
                scoreTracker.Configure(players.Count);
                scoreTracker.ResetScores();
            }

            BeginMatch();
        }

        public void BeginMatch()
        {
            SetPhase(MatchPhase.Playing);

            for (var i = 0; i < players.Count; i++)
            {
                if (players[i] != null)
                {
                    players[i].SetGameplayEnabled(true);
                }
            }

            if (matchTimer != null)
            {
                matchTimer.ResetTimer();
                matchTimer.StartTimer();
            }

            PublishHud();
        }

        public void StopMatchEarly()
        {
            if (Phase == MatchPhase.Completed)
            {
                return;
            }

            EndMatch(MatchEndReason.ExternalStop);
        }

        private void OnPlayerRingOut(PlayerController fallenPlayer)
        {
            if (Phase != MatchPhase.Playing || fallenPlayer == null)
            {
                return;
            }

            fallenPlayer.SetGameplayEnabled(false);

            var scoringPlayerIndex = ResolveOpponentIndex(fallenPlayer.PlayerIndex);
            if (scoringPlayerIndex >= 0 && scoreTracker != null)
            {
                scoreTracker.AddRingOutPoint(scoringPlayerIndex);
            }

            RingOutScored?.Invoke(fallenPlayer, scoringPlayerIndex);
            StartCoroutine(RespawnRoutine(fallenPlayer));
        }

        private IEnumerator RespawnRoutine(PlayerController player)
        {
            var body = player.GetComponent<Rigidbody2D>();
            var bodyCollider = player.GetComponent<Collider2D>();

            if (body != null)
            {
                body.linearVelocity = Vector2.zero;
                body.simulated = false;
            }

            if (bodyCollider != null)
            {
                bodyCollider.enabled = false;
            }

            yield return new WaitForSeconds(respawnDelaySeconds);

            var spawnPoint = ResolveSpawnPoint(player.PlayerIndex);
            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.position;
            }

            if (body != null)
            {
                body.simulated = true;
            }

            if (bodyCollider != null)
            {
                bodyCollider.enabled = true;
            }

            if (Phase == MatchPhase.Playing)
            {
                player.SetGameplayEnabled(true);
            }
        }

        private void OnTimerExpired()
        {
            if (Phase != MatchPhase.Playing)
            {
                return;
            }

            EndMatch(MatchEndReason.TimeExpired);
        }

        private void EndMatch(MatchEndReason reason)
        {
            SetPhase(MatchPhase.Completed);

            if (matchTimer != null)
            {
                matchTimer.StopTimer();
            }

            for (var i = 0; i < players.Count; i++)
            {
                if (players[i] != null)
                {
                    players[i].SetGameplayEnabled(false);
                }
            }

            var result = MatchResultCalculator.Calculate(
                scoreTracker != null ? scoreTracker.GetScore(0) : 0,
                players.Count > 1 && scoreTracker != null ? scoreTracker.GetScore(1) : 0,
                reason);
            var resultSummary = localTrophyProgress != null
                ? localTrophyProgress.ApplyMatchResult(result)
                : ResultSummaryGenerator.Create(result, 0);

            MatchEnded?.Invoke(result);
            ResultSummaryReady?.Invoke(resultSummary);
            TrophyIntegrationRequested?.Invoke("breakline_match_end", result);
            PublishHud(result);
        }

        private void OnTimeChanged(float _)
        {
            PublishHud();
        }

        private void OnScoreChanged(int _, int __)
        {
            PublishHud();
        }

        private void OnScoresReset()
        {
            PublishHud();
        }

        private void PublishHud()
        {
            if (hudBridge == null || scoreTracker == null || matchTimer == null)
            {
                return;
            }

            hudBridge.Publish(new BreaklineHudSnapshot(
                matchTimer.RemainingSeconds,
                scoreTracker.PlayerCount > 0 ? scoreTracker.GetScore(0) : 0,
                scoreTracker.PlayerCount > 1 ? scoreTracker.GetScore(1) : 0,
                Phase == MatchPhase.Completed,
                -1,
                string.Empty,
                0,
                Phase));
        }

        private void PublishHud(MatchResult result)
        {
            if (hudBridge == null || scoreTracker == null || matchTimer == null)
            {
                return;
            }

            hudBridge.Publish(new BreaklineHudSnapshot(
                matchTimer.RemainingSeconds,
                result.PlayerOneScore,
                result.PlayerTwoScore,
                true,
                result.WinnerPlayerIndex,
                string.Empty,
                0,
                Phase));
        }

        private void SetPhase(MatchPhase phase)
        {
            if (Phase == phase)
            {
                return;
            }

            Phase = phase;
            PhaseChanged?.Invoke(phase);
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

        private Transform ResolveSpawnPoint(int playerIndex)
        {
            for (var i = 0; i < spawnPoints.Count; i++)
            {
                if (spawnPoints[i] == null)
                {
                    continue;
                }

                if (i == playerIndex)
                {
                    return spawnPoints[i];
                }
            }

            return null;
        }
    }
}
