using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Data
{
    [CreateAssetMenu(menuName = "Data/Player Death Data")]
    public class PlayerDeathData : ScriptableObject
    {
        [Header("Timings (Unscaled)")]
        public float DesaturateDuration = 3;
        public float MoveToCenterDuration = 1.5f;
        public float PreLerpDelay = 4;
        public float LightFadeDuration = 0.3f;

        [Header("End Light Collapse")]
        public float EndLightDelay = 1.0f;          // NEW
        public float EndLightCloseDuration = 1.2f;  // NEW

        [Header("Camera Shake")]
        public float CameraShakeIntensity = 0.05f;
        public float CameraShakeDuration = 20;
        
        [Header("Player Shake")]
        public float PlayerShakeIntensity = 0.05f;
        public float PlayerShakeDuration = 20;

        [Header("Particles")]
        public float ParticleYOffset = 1f;

        [Header("Scene")]
        public string GameOverSceneName = "GameOver";
    }
}