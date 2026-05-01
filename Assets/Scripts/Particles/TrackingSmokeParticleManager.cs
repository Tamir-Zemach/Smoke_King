using UnityEngine;

namespace Particles
{
    public class TrackingSmokeParticleManager : MonoBehaviour
    {
        private ParticleSystem _parSystem;
        private ParticleSystemRenderer _parRenderer;

        private void GetComponents()
        {
            _parSystem = GetComponent<ParticleSystem>(); 
            _parRenderer = GetComponent<ParticleSystemRenderer>();
        }

        public void Init(Material material)
        {
            GetComponents();

            if (_parRenderer != null)
            {
                _parRenderer.material = material;
            }
        }

        public void ResetPos(Vector3 pos)
        {
            ParticleMovementUtility.ResetPosition(transform, pos);
        }
        
        public void SetDuration(float duration)
        {
            if (_parSystem == null)
                _parSystem = GetComponent<ParticleSystem>();

            // 1. Stop and clear particles
            _parSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            // 2. Modify duration
            var main = _parSystem.main;
            main.duration = duration;

            // 3. Restart the system
            _parSystem.Play();
        }

        
    }
}