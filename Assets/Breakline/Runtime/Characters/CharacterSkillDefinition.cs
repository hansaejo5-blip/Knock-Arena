using System;
using UnityEngine;

namespace Breakline.Runtime.Characters
{
    [Serializable]
    public struct CharacterSkillDefinition
    {
        [Tooltip("Designer-facing name shown in debug HUDs or selection UIs.")]
        public string displayName;

        [Tooltip("Prototype effect id. Hook real skill logic to this later.")]
        public CharacterSkillType skillType;

        [Tooltip("Use this to tune how often the skill should be available.")]
        public float cooldownSeconds;

        [Tooltip("General reach or radius for prototype skill logic.")]
        public float range;

        [Tooltip("Optional extra tile damage for floor-focused skills.")]
        public int tileDamageBonus;

        [Tooltip("Optional extra push strength for displacement skills.")]
        public float forceBonus;

        [Tooltip("Optional duration for zones, protection, or temporary effects.")]
        public float durationSeconds;
    }
}
