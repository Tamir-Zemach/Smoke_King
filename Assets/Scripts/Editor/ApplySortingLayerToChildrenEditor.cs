using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utilities;

[CustomEditor(typeof(ApplySortingLayerToChildren))]
public class ApplySortingLayerToChildrenEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        var script = (ApplySortingLayerToChildren)target;

        // Sorting layers dropdown
        var layers = SortingLayer.layers;
        string[] layerNames = new string[layers.Length];
        for (int i = 0; i < layers.Length; i++)
            layerNames[i] = layers[i].name;

        script.sortingLayerIndex = EditorGUILayout.Popup("Sorting Layer", script.sortingLayerIndex, layerNames);

        // Color field
        script.spriteColor = EditorGUILayout.ColorField("Sprite Color", script.spriteColor);

        // Shadow Caster LayerMask
        script.shadowTargetLayers = EditorGUILayout.LayerField("Shadow Target Layers", script.shadowTargetLayers);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Apply Sorting Layer"))
            script.ApplySortingLayer();

        if (GUILayout.Button("Apply Color"))
            script.ApplyColor();

        if (GUILayout.Button("Apply ShadowCaster2D Layers"))
            ApplyShadowCasterLayers(script);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("APPLY ALL"))
        {
            script.ApplySortingLayer();
            script.ApplyColor();
            ApplyShadowCasterLayers(script);
        }

        EditorUtility.SetDirty(script);
    }

    private void ApplyShadowCasterLayers(ApplySortingLayerToChildren script)
    {
        ShadowCaster2D[] casters = script.GetComponentsInChildren<ShadowCaster2D>(true);

        foreach (var caster in casters)
        {
            SerializedObject so = new SerializedObject(caster);
            SerializedProperty prop = so.FindProperty("m_ApplyToSortingLayers");

            if (prop != null)
            {
                prop.intValue = script.shadowTargetLayers.value;
                so.ApplyModifiedProperties();
            }
        }

        Debug.Log($"Applied ShadowCaster2D target layers to {casters.Length} casters.");
    }
}
