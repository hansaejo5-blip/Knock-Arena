using System;
using UnityEngine;

namespace Breakline.Runtime.Characters
{
    [Serializable]
    public struct CharacterTraitDefinition
    {
        [Tooltip("Prototype trait category. Hook passive logic to this later.")]
        public CharacterTraitType traitType;

        [Tooltip("Designer-facing summary for tuning and debugging.")]
        [TextArea(2, 3)]
        public string description;

        [Tooltip("Generic strength value for the passive trait.")]
        public float strength;

        [Tooltip("Generic duration value if the trait needs one later.")]
        public float durationSeconds;
    }
}
