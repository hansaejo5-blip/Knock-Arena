using Breakline.Runtime.Players;
using UnityEngine;

namespace Breakline.Runtime.Characters
{
    // Put this on the same GameObject as PlayerController.
    // Designers can swap CharacterDefinition assets in the Inspector and press Play.
    public sealed class CharacterLoadout : MonoBehaviour, ICharacterDefinitionProvider
    {
        [SerializeField] private CharacterDefinition startingCharacter;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private bool applyOnAwake = true;

        public CharacterDefinition CurrentCharacter { get; private set; }

        private void Reset()
        {
            playerController = GetComponent<PlayerController>();
        }

        private void Awake()
        {
            if (playerController == null)
            {
                playerController = GetComponent<PlayerController>();
            }

            if (applyOnAwake && startingCharacter != null)
            {
                ApplyCharacter(startingCharacter);
            }
        }

        public void ApplyCharacter(CharacterDefinition definition)
        {
            if (definition == null || playerController == null)
            {
                return;
            }

            CurrentCharacter = definition;
            playerController.ApplyCharacterTuning(definition.tuning);
        }

        public CharacterSkillDefinition GetSkill1()
        {
            return CurrentCharacter != null ? CurrentCharacter.skill1 : default;
        }

        public CharacterSkillDefinition GetSkill2()
        {
            return CurrentCharacter != null ? CurrentCharacter.skill2 : default;
        }

        public CharacterTraitDefinition GetTrait()
        {
            return CurrentCharacter != null ? CurrentCharacter.specialTrait : default;
        }
    }
}
