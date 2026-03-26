using UnityEngine;

namespace Breakline.Runtime.Players
{
    public sealed class BreaklineSpawnPoint : MonoBehaviour
    {
        [SerializeField] private int playerIndex;

        public int PlayerIndex => playerIndex;
    }
}
