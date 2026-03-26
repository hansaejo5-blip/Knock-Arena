using Breakline.Runtime.Characters;
using UnityEditor;
using UnityEngine;

namespace Breakline.Editor
{
    public static class PrototypeCharacterAssetGenerator
    {
        [MenuItem("Assets/Create/Breakline/Create 4 Prototype Characters", priority = 21)]
        public static void CreatePrototypeCharacters()
        {
            var folder = ResolveTargetFolder();

            var breaker = CreateCharacter(folder, "Breaker", "breaker", CharacterArchetype.Breaker, new CharacterTuningData
            {
                movementSpeed = 7f,
                dashCooldownSeconds = 0.95f,
                attackRange = 1.1f,
                attackForce = 11.5f,
                tileDamage = 2
            },
            new CharacterTraitDefinition
            {
                traitType = CharacterTraitType.BonusTileDamage,
                description = "Basic attacks are tuned to threaten floor integrity more than other prototypes.",
                strength = 1f,
                durationSeconds = 0f
            },
            new CharacterSkillDefinition
            {
                displayName = "Heavy Smash",
                skillType = CharacterSkillType.HeavySmash,
                cooldownSeconds = 5f,
                range = 1.2f,
                tileDamageBonus = 2,
                forceBonus = 2f,
                durationSeconds = 0f
            },
            new CharacterSkillDefinition
            {
                displayName = "Shockwave",
                skillType = CharacterSkillType.Shockwave,
                cooldownSeconds = 8f,
                range = 1.8f,
                tileDamageBonus = 1,
                forceBonus = 3f,
                durationSeconds = 0f
            });

            var chaser = CreateCharacter(folder, "Chaser", "chaser", CharacterArchetype.Chaser, new CharacterTuningData
            {
                movementSpeed = 9.5f,
                dashCooldownSeconds = 0.55f,
                attackRange = 1f,
                attackForce = 7f,
                tileDamage = 1
            },
            new CharacterTraitDefinition
            {
                traitType = CharacterTraitType.BonusDashRecovery,
                description = "Fast repositioning archetype with more frequent mobility windows.",
                strength = 1f,
                durationSeconds = 0f
            },
            new CharacterSkillDefinition
            {
                displayName = "Pursuit Dash",
                skillType = CharacterSkillType.PursuitDash,
                cooldownSeconds = 4f,
                range = 2.5f,
                tileDamageBonus = 0,
                forceBonus = 1f,
                durationSeconds = 0.15f
            },
            new CharacterSkillDefinition
            {
                displayName = "Pressure Burst",
                skillType = CharacterSkillType.PressureBurst,
                cooldownSeconds = 7f,
                range = 1.3f,
                tileDamageBonus = 0,
                forceBonus = 2f,
                durationSeconds = 0f
            });

            var controller = CreateCharacter(folder, "Controller", "controller", CharacterArchetype.Controller, new CharacterTuningData
            {
                movementSpeed = 8f,
                dashCooldownSeconds = 0.8f,
                attackRange = 1.3f,
                attackForce = 8.5f,
                tileDamage = 1
            },
            new CharacterTraitDefinition
            {
                traitType = CharacterTraitType.ZonePressure,
                description = "Excels at making sections of the arena awkward or dangerous to cross.",
                strength = 1f,
                durationSeconds = 0f
            },
            new CharacterSkillDefinition
            {
                displayName = "Denial Field",
                skillType = CharacterSkillType.DenialField,
                cooldownSeconds = 6f,
                range = 2f,
                tileDamageBonus = 1,
                forceBonus = 0f,
                durationSeconds = 3f
            },
            new CharacterSkillDefinition
            {
                displayName = "Trap Tile",
                skillType = CharacterSkillType.TrapTile,
                cooldownSeconds = 7.5f,
                range = 1.6f,
                tileDamageBonus = 1,
                forceBonus = 0f,
                durationSeconds = 4f
            });

            var engineer = CreateCharacter(folder, "Engineer", "engineer", CharacterArchetype.Engineer, new CharacterTuningData
            {
                movementSpeed = 7.75f,
                dashCooldownSeconds = 0.85f,
                attackRange = 1.05f,
                attackForce = 8f,
                tileDamage = 1
            },
            new CharacterTraitDefinition
            {
                traitType = CharacterTraitType.FloorProtection,
                description = "Has access to temporary floor utility and safer footing tools.",
                strength = 1f,
                durationSeconds = 2f
            },
            new CharacterSkillDefinition
            {
                displayName = "Reinforce Floor",
                skillType = CharacterSkillType.ReinforceFloor,
                cooldownSeconds = 6f,
                range = 1.3f,
                tileDamageBonus = 0,
                forceBonus = 0f,
                durationSeconds = 2.5f
            },
            new CharacterSkillDefinition
            {
                displayName = "Repair Patch",
                skillType = CharacterSkillType.RepairPatch,
                cooldownSeconds = 8f,
                range = 1.1f,
                tileDamageBonus = 0,
                forceBonus = 0f,
                durationSeconds = 0f
            });

            var library = ScriptableObject.CreateInstance<PrototypeCharacterLibrary>();
            library.characters.Add(breaker);
            library.characters.Add(chaser);
            library.characters.Add(controller);
            library.characters.Add(engineer);

            var libraryPath = AssetDatabase.GenerateUniqueAssetPath($"{folder}/PrototypeCharacterLibrary.asset");
            AssetDatabase.CreateAsset(library, libraryPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = library;
        }

        private static CharacterDefinition CreateCharacter(
            string folder,
            string displayName,
            string characterId,
            CharacterArchetype archetype,
            CharacterTuningData tuning,
            CharacterTraitDefinition trait,
            CharacterSkillDefinition skill1,
            CharacterSkillDefinition skill2)
        {
            var definition = ScriptableObject.CreateInstance<CharacterDefinition>();
            definition.displayName = displayName;
            definition.characterId = characterId;
            definition.archetype = archetype;
            definition.tuning = tuning;
            definition.specialTrait = trait;
            definition.skill1 = skill1;
            definition.skill2 = skill2;

            var path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{displayName}.asset");
            AssetDatabase.CreateAsset(definition, path);
            return definition;
        }

        private static string ResolveTargetFolder()
        {
            var selected = Selection.activeObject;
            if (selected == null)
            {
                return "Assets";
            }

            var path = AssetDatabase.GetAssetPath(selected);
            if (AssetDatabase.IsValidFolder(path))
            {
                return path;
            }

            var directory = System.IO.Path.GetDirectoryName(path);
            return string.IsNullOrWhiteSpace(directory) ? "Assets" : directory.Replace("\\", "/");
        }
    }
}
