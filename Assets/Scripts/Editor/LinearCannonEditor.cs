using Boss.BossAttacks;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(LinearCannon))]
    public class LinearCannonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var tracking = serializedObject.FindProperty("Tracking");
            var linearData = serializedObject.FindProperty("LinearCannonData");
            var trackingData = serializedObject.FindProperty("TrackingCannonData");
            var beam = serializedObject.FindProperty("Beam");
            var index = serializedObject.FindProperty("SpriteRendererIndex");

            // Always show these
            EditorGUILayout.PropertyField(beam);
            EditorGUILayout.PropertyField(index);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mode", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(tracking);

            if (tracking.boolValue)
            {
                // TRACKING MODE
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(trackingData);
                EditorGUI.indentLevel--;
            }
            else
            {
                // NORMAL MODE
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(linearData);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}