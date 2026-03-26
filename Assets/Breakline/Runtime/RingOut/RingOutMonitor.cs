using System;
using System.Collections.Generic;
using Breakline.Runtime.Players;
using UnityEngine;

namespace Breakline.Runtime.RingOut
{
    public sealed class RingOutMonitor : MonoBehaviour
    {
        [SerializeField] private List<BreaklinePlayerAvatar> trackedPlayers = new();

        private float _ringOutY;

        public event Action<BreaklinePlayerAvatar> PlayerRingOut;

        public void Initialize(float ringOutY)
        {
            _ringOutY = ringOutY;
        }

        private void Update()
        {
            for (var i = 0; i < trackedPlayers.Count; i++)
            {
                var player = trackedPlayers[i];
                if (player == null || player.IsRespawning)
                {
                    continue;
                }

                if (player.transform.position.y < _ringOutY)
                {
                    PlayerRingOut?.Invoke(player);
                }
            }
        }
    }
}
