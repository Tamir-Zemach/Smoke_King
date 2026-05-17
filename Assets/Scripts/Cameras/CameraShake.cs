using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;

namespace Cameras
{
    public class CameraShake : SingletonMonoBehaviour<CameraShake>
    {
        private CinemachineImpulseSource _impulseSource;
        private bool _isShaking = false;

        protected override void Awake()
        {
            base.Awake();
            _impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        /// <summary>
        /// Shake once (single impulse)
        /// </summary>
        public void Shake(float amplitude = 1f)
        {
            Vector3 impulseVelocity = Random.insideUnitSphere * amplitude;
            _impulseSource.GenerateImpulse(impulseVelocity);
        }

        /// <summary>
        /// Shake continuously for a duration
        /// </summary>
        public void Shake(float amplitude, float duration)
        {
            if (!_isShaking)
                StartCoroutine(ShakeRoutine(amplitude, duration));
        }

        private IEnumerator ShakeRoutine(float amplitude, float duration)
        {
            _isShaking = true;

            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;

                Vector3 impulseVelocity = Random.insideUnitSphere * amplitude;
                _impulseSource.GenerateImpulse(impulseVelocity);

                yield return null; // shake every frame
            }

            _isShaking = false;
        }
    }
}