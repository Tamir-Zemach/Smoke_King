using Enums;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Data;
using Utilities;

namespace Particles
{
    public class SmokeParticleManager : MonoBehaviour
    {
        public ParticleMovementData MovementData;

        private ParticleDamage2D _parDamage;
        private ParticleSystemRenderer _parRenderer;
        private Light2D _parLight;
        private ParticleSystem _parSystem;

        private void GetParticleComponents()
        {
            _parSystem = GetComponent<ParticleSystem>();
            _parDamage = GetComponent<ParticleDamage2D>();
            _parRenderer = GetComponent<ParticleSystemRenderer>();
            _parLight = GetComponentInChildren<Light2D>();
        }

        public void Init(Color color, Material material, StateType state)
        {
            GetParticleComponents();

            if (_parRenderer != null)
                _parRenderer.material = material;

            if (_parLight != null)
                _parLight.color = color;

            if (_parDamage != null)
                _parDamage.StateType = state;

            SetRateOverDistance();
        }

        public void ResetPosition(Vector3 pos)
        {
            ParticleMovementUtility.ResetPosition(transform, pos);
        }

        public void MoveInCircle(float duration)
        {
            ParticleMovementUtility.MoveInCircle(
                transform,
                MovementData.CircleRadius,
                MovementData.CircleSpeed,
                duration
            );
        }




        public void Fly()
        {
            ParticleMovementUtility.Fly(
                transform,
                MovementData.FlyDistance,
                MovementData.FlyDuration
            );
        }

        private void SetRateOverDistance()
        {
            if (_parSystem == null)
                GetParticleComponents();

            var emission = _parSystem.emission;
            var rate = emission.rateOverDistance;

            rate.mode = ParticleSystemCurveMode.TwoConstants;
            rate.constantMin = MovementData.RateOverDistanceMin;
            rate.constantMax = MovementData.RateOverDistanceMax;

            emission.rateOverDistance = rate;
        }
    }
}
