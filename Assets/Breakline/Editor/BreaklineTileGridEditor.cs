using Breakline.Runtime.Tiles;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Breakline.Editor
{
    [CustomEditor(typeof(BreaklineTileGrid))]
    public sealed class BreaklineTileGridEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            if (GUILayout.Button("Rebuild Tile Grid"))
            {
                var grid = (BreaklineTileGrid)target;
                grid.RebuildImmediate();
                EditorSceneManager.MarkSceneDirty(grid.gameObject.scene);
            }
        }
    }
}
