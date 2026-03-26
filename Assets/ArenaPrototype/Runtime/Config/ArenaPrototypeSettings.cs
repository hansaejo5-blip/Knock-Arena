using UnityEngine;

namespace ArenaPrototype.Runtime.Config
{
    [CreateAssetMenu(
        fileName = "ArenaPrototypeSettings",
        menuName = "Arena Prototype/Config/Prototype Settings")]
    public sealed class ArenaPrototypeSettings : ScriptableObject
    {
        [Header("Match")]
        [Min(10f)]
        public float matchDurationSeconds = 120f;

        [Min(0f)]
        public float respawnDelaySeconds = 1.25f;

        public float ringOutY = -6f;

        [Header("Arena")]
        public LayerMask playerLayerMask;
        public LayerMask floorLayerMask;
    }
}
