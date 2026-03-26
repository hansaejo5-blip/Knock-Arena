using ArenaPrototype.Runtime.Environment;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ArenaPrototype.Editor
{
    [CustomEditor(typeof(ArenaFloorGrid))]
    public sealed class ArenaFloorGridEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            if (GUILayout.Button("Rebuild Grid"))
            {
                var grid = (ArenaFloorGrid)target;
                grid.RebuildImmediate();
                EditorSceneManager.MarkSceneDirty(grid.gameObject.scene);
            }
        }
    }
}
