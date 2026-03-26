using Breakline.Runtime.Config;
using UnityEditor;
using UnityEngine;

namespace Breakline.Editor
{
    public static class BreaklineAssetMenu
    {
        [MenuItem("Assets/Create/Breakline/Create Default Config Assets", priority = 20)]
        public static void CreateDefaultConfigs()
        {
            var folder = ResolveTargetFolder();
            CreateAsset<BreaklineMatchSettings>(folder, "BreaklineMatchSettings.asset");
            CreateAsset<BreaklinePlayerSettings>(folder, "BreaklinePlayerSettings.asset");
            CreateAsset<BreaklineTileSettings>(folder, "BreaklineTileSettings.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateAsset<T>(string folder, string fileName) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            var path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{fileName}");
            AssetDatabase.CreateAsset(asset, path);
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
