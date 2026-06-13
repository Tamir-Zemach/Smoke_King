using UnityEngine;

namespace Cameras
{
    public class ForceAspect : MonoBehaviour
    {
        private readonly float _targetAspect = 16f / 9f;

        void Start()
        {
#if UNITY_WEBGL
            // Disable the script entirely in WebGL
            enabled = false;
            return;
#endif

            float windowAspect = (float)Screen.width / Screen.height;
            float scaleHeight = windowAspect / _targetAspect;

            Camera cam = GetComponent<Camera>();

            if (scaleHeight < 1.0f)
            {
                Rect rect = cam.rect;

                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;

                cam.rect = rect;
            }
            else
            {
                float scaleWidth = 1.0f / scaleHeight;

                Rect rect = cam.rect;

                rect.width = scaleWidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scaleWidth) / 2.0f;
                rect.y = 0;

                cam.rect = rect;
            }
        }
    }
}