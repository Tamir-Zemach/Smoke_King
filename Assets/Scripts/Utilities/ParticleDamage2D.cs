using System;
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

            print($"collided with {other.name}");

            // If it hits ANYTHING in HitLayer → stop the particle system
            _onFinish?.Invoke();
            ParticleMovementUtility.KillTweens(transform);

            // Try to damage if possible
            if (other.TryGetComponent<IDamageable>(out var dmg))
            {
                if (!dmg.IsSameState(_stateType) && !dmg.IsInvincible())
                {
                    dmg.TakeDamage(Damage, _stateType);
                }
            }

            // Play impact only once
            if (!_impactPlayed && ImpactParticlePool.Instance != null)
            {
                _impactPlayed = true;
                ImpactParticlePool.Instance.PlayImpact(transform.position, _material, _lightColor);
            }
        }



    }
}