using UnityEngine;

namespace Background
{
    public class ParallaxManager : MonoBehaviour
    {
        [System.Serializable]
        public class ParallaxLayer
        {
            public Transform LayerTransform;

            [Header("Manual Control")]
            [Tooltip("Used only when Auto Speed is OFF")]
            public float ManualSpeed = 0.5f;

            [HideInInspector] public float DepthFromCamera;
            [HideInInspector] public float AutoSpeed;
        }

        [Header("References")]
        public Transform CameraTransform;

        [Header("Layers")]
        public ParallaxLayer[] Layers;

        [Header("Parallax Settings")]
        [Range(0f, 2f)]
        public float SpeedMultiplier = 0.5f;

        [Tooltip("Automatically calculate speed based on Z depth")]
        public bool UseAutoSpeed = true;

        [Header("Axis Control")]
        public bool AffectX = true;
        public bool AffectY = true;

        private Vector3 _lastCameraPosition;

        private void Start()
        {
            if (CameraTransform == null)
            {
                Camera mainCam = Camera.main;
                if (mainCam != null)
                    CameraTransform = mainCam.transform;
                else
                {
                    Debug.LogError("ParallaxManager: No camera assigned and no Main Camera found.");
                    enabled = false;
                    return;
                }
            }

            _lastCameraPosition = CameraTransform.position;
            CalculateLayerSpeeds();
        }

        private void LateUpdate()
        {
            if (CameraTransform == null || Layers == null || Layers.Length == 0)
                return;

            Vector3 cameraDelta = CameraTransform.position - _lastCameraPosition;

            for (int i = 0; i < Layers.Length; i++)
            {
                if (Layers[i] == null || Layers[i].LayerTransform == null)
                    continue;

                float speed = UseAutoSpeed ? Layers[i].AutoSpeed : Layers[i].ManualSpeed;

                float moveX = AffectX ? cameraDelta.x * speed * SpeedMultiplier : 0f;
                float moveY = AffectY ? cameraDelta.y * speed * SpeedMultiplier : 0f;

                Layers[i].LayerTransform.position -= new Vector3(moveX, moveY, 0f);
            }

            _lastCameraPosition = CameraTransform.position;
        }

        private void CalculateLayerSpeeds()
        {
            if (Layers == null || Layers.Length == 0 || CameraTransform == null)
                return;

            float minDepth = float.MaxValue;
            float maxDepth = float.MinValue;

            // Get depth values
            for (int i = 0; i < Layers.Length; i++)
            {
                if (Layers[i] == null || Layers[i].LayerTransform == null)
                    continue;

                float depth = Mathf.Abs(Layers[i].LayerTransform.position.z - CameraTransform.position.z);
                Layers[i].DepthFromCamera = depth;

                if (depth < minDepth) minDepth = depth;
                if (depth > maxDepth) maxDepth = depth;
            }

            float range = maxDepth - minDepth;

            // Normalize speeds
            for (int i = 0; i < Layers.Length; i++)
            {
                if (Layers[i] == null || Layers[i].LayerTransform == null)
                    continue;

                if (range <= 0.001f)
                {
                    Layers[i].AutoSpeed = 1f;
                }
                else
                {
                    float normalized = Mathf.InverseLerp(maxDepth, minDepth, Layers[i].DepthFromCamera);
                    Layers[i].AutoSpeed = normalized;
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                if (CameraTransform == null && Camera.main != null)
                    CameraTransform = Camera.main.transform;

                CalculateLayerSpeeds();
            }
        }
#endif
    }
}