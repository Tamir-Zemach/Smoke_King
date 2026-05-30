using Enums;
using Interfaces;
using UnityEngine;

namespace Utilities
{
    public class DamageGiver : MonoBehaviour
    {
        public StateType StateType = StateType.State1;
        public int Damage = 1;
        public LayerMask HitLayer;

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!IsInLayerMask(other.gameObject, HitLayer)) 
                return;

            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                // Compute hit point
                Vector3 hitPoint = other.ClosestPoint(transform.position);

                // Call the overload with hit point
                damageable.TakeDamage(Damage, StateType, hitPoint);
            }
        }

        private bool IsInLayerMask(GameObject obj, LayerMask mask)
        {
            var objectLayerBit = 1 << obj.layer;
            return (mask.value & objectLayerBit) != 0;
        }
    }
}