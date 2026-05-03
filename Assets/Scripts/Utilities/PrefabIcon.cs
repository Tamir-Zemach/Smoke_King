using UnityEngine;

namespace Utilities
{
    public class PrefabIcon : MonoBehaviour
    {
        public Sprite Icon;

        public Texture2D Texture => Icon != null ? Icon.texture : null;

        public Rect UV
        {
            get
            {
                if (Icon == null) return Rect.zero;

                // Convert sprite rect (pixels) → UV (0–1)
                Rect r = Icon.rect;
                return new Rect(
                    r.x / Icon.texture.width,
                    r.y / Icon.texture.height,
                    r.width / Icon.texture.width,
                    r.height / Icon.texture.height
                );
            }
        }
    }
}