using UnityEngine;

namespace Breakline.Runtime.Config
{
    [CreateAssetMenu(fileName = "BreaklineMatchSettings", menuName = "Breakline/Config/Match Settings")]
    public sealed class BreaklineMatchSettings : ScriptableObject
    {
        [Min(10f)] public float matchDurationSeconds = 120f;
        [Min(0f)] public float respawnDelaySeconds = 1.25f;
        public float ringOutY = -7f;
        public LayerMask playerLayerMask;
        public LayerMask floorLayerMask;
    }
}
