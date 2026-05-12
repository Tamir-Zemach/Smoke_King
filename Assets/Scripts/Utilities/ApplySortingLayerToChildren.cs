using UnityEngine;

namespace Utilities
{
    public class ApplySortingLayerToChildren : MonoBehaviour
    {
        [Header("Sorting Layer Settings")]
        public int sortingLayerIndex = 0;

        [Header("Color Settings")]
        public Color spriteColor = Color.white;

        [Header("Shadow Caster Settings")]
        public LayerMask shadowTargetLayers;

        public void ApplySortingLayer()
        {
            var layers = SortingLayer.layers;
            if (sortingLayerIndex < 0 || sortingLayerIndex >= layers.Length)
            {
                Debug.LogError("Invalid sorting layer index");
                return;
            }

            string layerName = layers[sortingLayerIndex].name;

            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

            foreach (var sr in renderers)
                sr.sortingLayerName = layerName;

            Debug.Log($"Applied sorting layer '{layerName}' to {renderers.Length} SpriteRenderers.");
        }

        public void ApplyColor()
        {
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

            foreach (var sr in renderers)
                sr.color = spriteColor;

            Debug.Log($"Applied color {spriteColor} to {renderers.Length} SpriteRenderers.");
        }

        public void ApplyAll()
        {
            ApplySortingLayer();
            ApplyColor();
            // ShadowCaster2D layers are handled in editor script only
        }
    }
}