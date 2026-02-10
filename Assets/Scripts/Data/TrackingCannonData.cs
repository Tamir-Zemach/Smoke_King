using UnityEngine;

namespace Data
{
    [CreateAssetMenu]
    public class TrackingCannonData : CannonAttackData
    {
        [Header("Tracking Parameters")] public float TrackingDuration = 2f;

        public float LockDuration = 0.2f;
        public float RotationSpeed = 180f;
    }
}