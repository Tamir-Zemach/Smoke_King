using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Data/Particle Movement Data")]
    public class ParticleMovementData : ScriptableObject
    {
        [Header("MoveInCircle Settings")]
        public float CircleRadius = 2f;
        [Range(1f, 360f)]
        public float CircleSpeed = 180f; // degrees per second

        
        
        [Header("Shake Settings")]
        public float ShakeDuration = 0.3f;
        public float ShakeStrength = 0.2f;
        public int ShakeVibrato = 20;

        [Header("Fly Settings")]
        public float FlyDistance = 5f;
        public float FlyDuration = 0.5f;

        [Header("Emission Settings")]
        public float RateOverDistanceMin = 7f;
        public float RateOverDistanceMax = 12f;
    }
}