using Boss.BossAttacks;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LinearCannon))]
    public class LinearCannonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw the script reference (read‑only)
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            GUI.enabled = true;

            var tracking = serializedObject.FindProperty("Tracking");
            var linearData = serializedObject.FindProperty("LinearCannonData");
            var trackingData = serializedObject.FindProperty("TrackingCannonData");
            var smokeParticle = serializedObject.FindProperty("SmokeParticle");
            var trackingSmokeParticle = serializedObject.FindProperty("TrackingSmokeParticleSmokeParticle");

            // Always show SmokeParticle
            EditorGUILayout.PropertyField(smokeParticle);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mode", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(tracking);

            if (tracking.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(trackingData);
                EditorGUILayout.PropertyField(trackingSmokeParticle);
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(linearData);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}