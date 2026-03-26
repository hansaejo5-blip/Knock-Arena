using System;
using Breakline.Runtime.Players;
using UnityEngine;

namespace Breakline.Runtime.Match
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class RingOutDetector : MonoBehaviour
    {
        public event Action<PlayerController> PlayerRingOut;

        private void Reset()
        {
            var ownCollider = GetComponent<Collider2D>();
            ownCollider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponentInParent<PlayerController>();
            if (player == null)
            {
                return;
            }

            PlayerRingOut?.Invoke(player);
        }
    }
}
