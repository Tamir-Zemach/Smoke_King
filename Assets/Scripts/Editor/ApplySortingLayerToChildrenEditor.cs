using UnityEditor;
using UnityEngine;
using Utilities;

[CustomEditor(typeof(ApplySortingLayerToChildren))]
public class ApplySortingLayerToChildrenEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        var script = (ApplySortingLayerToChildren)target;

        // Get all sorting layers
        var layers = SortingLayer.layers;
        string[] layerNames = new string[layers.Length];
        for (int i = 0; i < layers.Length; i++)
            layerNames[i] = layers[i].name;

        // Dropdown
        script.sortingLayerIndex = EditorGUILayout.Popup("Sorting Layer", script.sortingLayerIndex, layerNames);

        // Button
        if (GUILayout.Button("Apply Sorting Layer To Children"))
        {
            script.ApplyLayer();
        }

        EditorUtility.SetDirty(script);
    }
}