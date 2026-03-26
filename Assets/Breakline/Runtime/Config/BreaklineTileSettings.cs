using UnityEngine;

namespace Breakline.Runtime.Config
{
    [CreateAssetMenu(fileName = "BreaklineTileSettings", menuName = "Breakline/Config/Tile Settings")]
    public sealed class BreaklineTileSettings : ScriptableObject
    {
        [Min(1)] public int maxDurability = 2;
    }
}
