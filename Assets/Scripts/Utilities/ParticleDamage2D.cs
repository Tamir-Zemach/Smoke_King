using System;
using Enums;
using Interfaces;
using ObjectPooling;
using Particles;
using Unity.Profiling;
using UnityEngine;

namespace Utilities
{
    public class ParticleDamage2D : MonoBehaviour
    {
        public LayerMask HitLayer;
        public int Damage = 1;

        private Material _material;
        private StateType _stateType;
        private Action _onFinish;
        private Color _lightColor;

        private bool _hasHit;

        private static readonly ProfilerMarker ParticleCollisionMarker =
            new ProfilerMarker("SmokeKing.ParticleDamage2D.OnParticleCollision");

        public void Init(StateType state, Material material, Color color, Action onFinished = null)
        {
            _stateType = state;
            _material = material;
            _onFinish = onFinished;
            _lightColor = color;

            _hasHit = false;
        }

        private void OnParticleCollision(GameObject other)
        {
            using (ParticleCollisionMarker.Auto())
            {
                if (_hasHit)
                    return;

                if ((HitLayer.value & (1 << other.layer)) == 0)
                    return;

                if (!other.TryGetComponent<IDamageable>(out var dmg))
                    return;

                _hasHit = true;

                _onFinish?.Invoke();
                ParticleMovementUtility.KillTweens(transform);

                dmg.TakeDamage(Damage, _stateType);

                if (ImpactParticlePool.Instance != null)
                {
                    ImpactParticlePool.Instance.PlayImpact(transform.position, _material, _lightColor);
                }
            }
        }
    }
}