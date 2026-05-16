using Unity.Cinemachine;
using UnityEngine;
using Utilities;

namespace Cameras
{
    public class CameraShake : SingletonMonoBehaviour<CameraShake>
    {
        private CinemachineImpulseSource _impulseSource;

        protected override void Awake()
        {
            base.Awake();
            _impulseSource = GetComponent<CinemachineImpulseSource>();

            if (_impulseSource == null)
                Debug.LogError("CameraShake requires a CinemachineImpulseSource component.");
        }

        /// <summary>
        /// Shake the camera using Cinemachine Impulse (v3 compatible)
        /// </summary>
        public void Shake(float amplitude = 1f)
        {
            // The direction doesn't matter for screen shake, only magnitude
            Vector3 impulseVelocity = Random.insideUnitSphere * amplitude;

            _impulseSource.GenerateImpulse(impulseVelocity);
        }
    }
}