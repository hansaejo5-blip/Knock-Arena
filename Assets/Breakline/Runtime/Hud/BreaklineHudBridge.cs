using System;
using UnityEngine;

namespace Breakline.Runtime.Hud
{
    public sealed class BreaklineHudBridge : MonoBehaviour
    {
        public event Action<BreaklineHudSnapshot> SnapshotPublished;

        public BreaklineHudSnapshot Current { get; private set; }

        public void Publish(BreaklineHudSnapshot snapshot)
        {
            Current = snapshot;
            SnapshotPublished?.Invoke(snapshot);
        }
    }
}
