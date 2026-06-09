using System;
using Audio;
using Enums;
using Interfaces;
using ObjectPooling;
using Particles;
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

        private bool _impactPlayed = false;  

        public void Init(StateType state, Material material, Color color, Action onFinished = null)
        {
            _stateType = state;
            _material = material;
            _onFinish = onFinished;
            _lightColor = color;
            _impactPlayed = false;            
        }

        void OnParticleCollision(GameObject other)
        {
            if ((HitLayer.value & (1 << other.layer)) == 0)
                return;

            _onFinish?.Invoke();
            ParticleMovementUtility.KillTweens(transform);

            if (other.TryGetComponent<IDamageable>(out var dmg))
            {
                    dmg.TakeDamage(Damage, _stateType);
            }

            if (!_impactPlayed && ImpactParticlePool.Instance != null)
            {
                _impactPlayed = true;
                ImpactParticlePool.Instance.PlayImpact(transform.position, _material, _lightColor);
            }
        }




    }
}