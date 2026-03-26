using UnityEngine;

namespace ArenaPrototype.Runtime.Players
{
    public sealed class ArenaPlayerSpawnPoint : MonoBehaviour
    {
        [SerializeField] private int playerIndex;

        public int PlayerIndex => playerIndex;
    }
}
