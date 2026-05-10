using UnityEngine;

namespace Utilities
{
    public class ApplySortingLayerToChildren : MonoBehaviour
    {
        [Header("Sorting Layer Settings")]
        public int sortingLayerIndex = 0;   // index into SortingLayer.layers

        [ContextMenu("Apply Sorting Layer To Children")]
        public void ApplyLayer()
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
            {
                sr.sortingLayerName = layerName;
            }

            Debug.Log($"Applied sorting layer '{layerName}' to {renderers.Length} SpriteRenderers.");
        }
    }
}