using UnityEngine;

namespace Utilities
{
    public class SpriteChildrenConfigurator : MonoBehaviour
    {
        [Header("Sorting Layer Settings")]
        public int SortingLayerIndex = 0;

        [Header("Color Settings")]
        public Color SpriteColor = Color.white;

        [Header("Shadow Caster Settings (Sorting Layers)")]
        public int ShadowSortingLayerMask = 0;

        [Header("Material Settings")]
        public Material OverrideMaterial;

        public void ApplySortingLayer()
        {
            var layers = SortingLayer.layers;
            if (SortingLayerIndex < 0 || SortingLayerIndex >= layers.Length)
            {
                Debug.LogError("Invalid sorting layer index");
                return;
            }

            string layerName = layers[SortingLayerIndex].name;

            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

            foreach (var sr in renderers)
                sr.sortingLayerName = layerName;

            Debug.Log($"Applied sorting layer '{layerName}' to {renderers.Length} SpriteRenderers.");
        }

        public void ApplyColor()
        {
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

            foreach (var sr in renderers)
                sr.color = SpriteColor;

            Debug.Log($"Applied color {SpriteColor} to {renderers.Length} SpriteRenderers.");
        }

        public void ApplyMaterial()
        {
            if (OverrideMaterial == null)
            {
                Debug.LogWarning("No material assigned to SpriteChildrenConfigurator.overrideMaterial");
                return;
            }

            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

            foreach (var sr in renderers)
                sr.sharedMaterial = OverrideMaterial;

            Debug.Log($"Applied material '{OverrideMaterial.name}' to {renderers.Length} SpriteRenderers.");
        }

        public void ApplyAll()
        {
            ApplySortingLayer();
            ApplyColor();
            ApplyMaterial();
        }
    }
}
