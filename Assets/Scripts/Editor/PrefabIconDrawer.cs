using UnityEditor;
using UnityEngine;
using Utilities;

namespace Editor
{
    [InitializeOnLoad]
    public static class PrefabIconDrawer
    {
        static PrefabIconDrawer()
        {
            EditorApplication.projectWindowItemOnGUI += DrawIcon;
        }

        static void DrawIcon(string guid, Rect rect)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null) return;

            var iconHolder = prefab.GetComponent<PrefabIcon>();
            if (iconHolder == null || iconHolder.Icon == null) return;

            Texture2D tex = iconHolder.Texture;
            if (tex == null) return;

            Rect uv = iconHolder.UV;

            // --- TRUE AUTO SCALE BASED ON RECT ---
            float size = Mathf.Min(rect.width, rect.height) * 0.6f;
            size = Mathf.Clamp(size, 16f, 128f);

            // --- UNITY-STYLE OFFSET ---
            // Unity pushes thumbnails slightly upward (~28%)
            float x = rect.x + (rect.width - size) * 0.5f;
            float y = rect.y + (rect.height - size) * 0.35f;

            Rect iconRect = new Rect(x, y, size, size);

            GUI.DrawTextureWithTexCoords(iconRect, tex, uv);
        }
    }
}