using DG.Tweening;
using UnityEngine;

namespace Cameras
{
    public class CameraZoom : MonoBehaviour
    {
        [Header("Zoom Settings")]
        public float ZoomInSize = 3f;
        public float ZoomOutSize = 6f;
        public float Duration = 1f;

        [Header("Loop Settings")]
        public bool Loop = true;
        public LoopType LoopType = LoopType.Yoyo;

        private Camera _cam;

        void Start()
        {
            _cam = GetComponent<Camera>();

            // Start zoom tween
            Tween zoomTween = _cam.DOOrthoSize(ZoomInSize, Duration);

            if (Loop)
            {
                zoomTween.SetLoops(-1, LoopType);
            }
        }
    }
}