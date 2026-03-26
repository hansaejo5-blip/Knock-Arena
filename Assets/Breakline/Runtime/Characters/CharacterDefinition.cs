using UnityEngine;

namespace Breakline.Runtime.Characters
{
    [CreateAssetMenu(fileName = "CharacterDefinition", menuName = "Breakline/Characters/Character Definition")]
    public sealed class CharacterDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string characterId;
        public string displayName;
        public CharacterArchetype archetype;

        [Header("Shared Controller Tuning")]
        public CharacterTuningData tuning;

        [Header("Trait")]
        public CharacterTraitDefinition specialTrait;

        [Header("Skills")]
        public CharacterSkillDefinition skill1;
        public CharacterSkillDefinition skill2;
    }
}
