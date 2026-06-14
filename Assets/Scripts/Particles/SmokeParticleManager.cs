using Audio;
using Enums;
using UnityEngine;
using Data;
using Utilities;

namespace Particles
{
    public class SmokeParticleManager : MonoBehaviour
    {
        public ParticleMovementData MovementData;

        [Header("Material Testing")]
        [SerializeField] private bool _useSharedMaterial = true;

        private ParticleDamage2D _parDamage;
        private ParticleSystemRenderer _parRenderer;
        private ParticleSystem _parSystem;

        private void Awake()
        {
            CacheParticleComponents();
        }

        private void CacheParticleComponents()
        {
            _parSystem = GetComponent<ParticleSystem>();
            _parDamage = GetComponent<ParticleDamage2D>();
            _parRenderer = GetComponent<ParticleSystemRenderer>();
        }

        private void EnsureComponents()
        {
            if (_parSystem == null)
                _parSystem = GetComponent<ParticleSystem>();

            if (_parDamage == null)
                _parDamage = GetComponent<ParticleDamage2D>();

            if (_parRenderer == null)
                _parRenderer = GetComponent<ParticleSystemRenderer>();
        }

        public void Init(Color color, Material material, StateType state)
        {
            EnsureComponents();

            if (_parSystem != null)
            {
                var collision = _parSystem.collision;
                collision.enabled = true;
            }

            if (_parRenderer != null)
            {
                if (_useSharedMaterial)
                {
                    _parRenderer.sharedMaterial = material;
                }
                else
                {
                    _parRenderer.material = material;
                }
            }

            if (_parDamage != null)
            {
                _parDamage.Init(state, material, color, Stop);
            }

            SetRateOverDistance();
        }

        public void ResetPos(Vector3 pos)
        {
            ParticleMovementUtility.ResetPosition(transform, pos);
        }

        private void Stop()
        {
            EnsureComponents();

            if (_parSystem != null)
            {
                // Stop new emission, but let existing particles fade naturally.
                _parSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                // Disable collision after a valid hit so fading particles do not keep firing callbacks.
                var collision = _parSystem.collision;
                collision.enabled = false;
            }
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
            AudioManager.Instance.PlaySfx(SfxType.SmokeAttackWhoosh, canOverlap: false);

            ParticleMovementUtility.Fly(
                transform,
                MovementData.FlyDistance,
                MovementData.FlyDuration
            );
        }

        private void SetRateOverDistance()
        {
            EnsureComponents();

            if (_parSystem == null)
                return;

            var emission = _parSystem.emission;
            var rate = emission.rateOverDistance;

            rate.mode = ParticleSystemCurveMode.TwoConstants;
            rate.constantMin = MovementData.RateOverDistanceMin;
            rate.constantMax = MovementData.RateOverDistanceMax;

            emission.rateOverDistance = rate;
        }
    }
}