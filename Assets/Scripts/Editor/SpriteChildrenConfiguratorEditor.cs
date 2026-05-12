using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utilities;

namespace Editor
{
    [CustomEditor(typeof(SpriteChildrenConfigurator))]
    public class SpriteChildrenConfiguratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var script = (SpriteChildrenConfigurator)target;

            // Sorting layers dropdown
            var layers = SortingLayer.layers;
            string[] layerNames = new string[layers.Length];
            for (int i = 0; i < layers.Length; i++)
                layerNames[i] = layers[i].name;

            script.SortingLayerIndex = EditorGUILayout.Popup("Sorting Layer", script.SortingLayerIndex, layerNames);

            // Color field
            script.SpriteColor = EditorGUILayout.ColorField("Sprite Color", script.SpriteColor);

            // Material field
            script.OverrideMaterial = (Material)EditorGUILayout.ObjectField("Override Material", script.OverrideMaterial, typeof(Material), false);

            // Sorting Layer Mask for ShadowCaster2D
            script.ShadowSortingLayerMask = SortingLayerMaskField("Shadow Target Sorting Layers", script.ShadowSortingLayerMask);

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Apply Sorting Layer"))
                script.ApplySortingLayer();

            if (GUILayout.Button("Apply Color"))
                script.ApplyColor();

            if (GUILayout.Button("Apply Material"))
                script.ApplyMaterial();

            if (GUILayout.Button("Apply ShadowCaster2D Layers"))
                ApplyShadowCasterLayers(script);

            EditorGUILayout.Space(10);

            if (GUILayout.Button("APPLY ALL"))
            {
                script.ApplySortingLayer();
                script.ApplyColor();
                script.ApplyMaterial();
                ApplyShadowCasterLayers(script);
            }

            EditorUtility.SetDirty(script);
        }

        private void ApplyShadowCasterLayers(SpriteChildrenConfigurator script)
        {
            ShadowCaster2D[] casters = script.GetComponentsInChildren<ShadowCaster2D>(true);

            foreach (var caster in casters)
            {
                SerializedObject so = new SerializedObject(caster);
                SerializedProperty prop = so.FindProperty("m_ApplyToSortingLayers");

                if (prop != null)
                {
                    prop.intValue = script.ShadowSortingLayerMask;
                    so.ApplyModifiedProperties();
                }
            }

            Debug.Log($"Applied ShadowCaster2D target sorting layers to {casters.Length} casters.");
        }

        // Draws a Sorting Layer Mask (Unity does NOT provide this)
        public static int SortingLayerMaskField(string label, int mask)
        {
            var layers = SortingLayer.layers;
            string[] names = new string[layers.Length];

            for (int i = 0; i < layers.Length; i++)
                names[i] = layers[i].name;

            return EditorGUILayout.MaskField(label, mask, names);
        }
    }
}
