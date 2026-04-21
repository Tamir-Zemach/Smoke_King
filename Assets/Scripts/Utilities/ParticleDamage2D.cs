using Enums;
using Interfaces;
using UnityEngine;

namespace Utilities
{
    public class ParticleDamage2D : MonoBehaviour
    {
        public StateType StateType = StateType.State1;
        public int Damage = 1;
        public LayerMask HitLayer;

        void OnParticleCollision(GameObject other)
        {
            if ((HitLayer.value & (1 << other.layer)) == 0)
                return;

            if (other.TryGetComponent<IDamageable>(out var dmg))
            {
                print($"{other.name} is getting hit by {transform.name}");
                dmg.TakeDamage(Damage, StateType);
            }
        }
    }
}