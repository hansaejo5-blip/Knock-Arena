using ArenaPrototype.Runtime.Config;
using UnityEditor;
using UnityEngine;

namespace ArenaPrototype.Editor
{
    public static class ArenaPrototypeAssetMenu
    {
        [MenuItem("Assets/Create/Arena Prototype/Create Default Prototype Configs", priority = 20)]
        public static void CreateDefaultConfigs()
        {
            var targetFolder = GetSelectedFolderOrAssetsRoot();

            CreateAsset<ArenaPrototypeSettings>(targetFolder, "ArenaPrototypeSettings.asset");
            CreateAsset<PlayerCombatSettings>(targetFolder, "PlayerCombatSettings.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateAsset<T>(string folder, string fileName) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            var assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{fileName}");
            AssetDatabase.CreateAsset(asset, assetPath);
        }

        private static string GetSelectedFolderOrAssetsRoot()
        {
            var path = "Assets";
            var selected = Selection.activeObject;

            if (selected == null)
            {
                return path;
            }

            var selectedPath = AssetDatabase.GetAssetPath(selected);
            if (string.IsNullOrWhiteSpace(selectedPath))
            {
                return path;
            }

            if (AssetDatabase.IsValidFolder(selectedPath))
            {
                return selectedPath;
            }

            var directory = System.IO.Path.GetDirectoryName(selectedPath);
            return string.IsNullOrWhiteSpace(directory) ? path : directory.Replace("\\", "/");
        }
    }
}
