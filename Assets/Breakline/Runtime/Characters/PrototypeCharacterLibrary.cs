using System.Collections.Generic;
using UnityEngine;

namespace Breakline.Runtime.Characters
{
    [CreateAssetMenu(fileName = "PrototypeCharacterLibrary", menuName = "Breakline/Characters/Prototype Character Library")]
    public sealed class PrototypeCharacterLibrary : ScriptableObject
    {
        [Tooltip("Convenience list for menus, selection screens, or quick debug spawning.")]
        public List<CharacterDefinition> characters = new();
    }
}
