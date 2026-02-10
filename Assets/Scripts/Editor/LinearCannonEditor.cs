using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(Boss.BossAttacks.LinearCannon))]
    public class LinearCannonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty tracking = serializedObject.FindProperty("Tracking");
            SerializedProperty linearData = serializedObject.FindProperty("LinearCannonData");
            SerializedProperty trackingData = serializedObject.FindProperty("TrackingCannonData");
            SerializedProperty beam = serializedObject.FindProperty("Beam");
            SerializedProperty index = serializedObject.FindProperty("SpriteRendererIndex");

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