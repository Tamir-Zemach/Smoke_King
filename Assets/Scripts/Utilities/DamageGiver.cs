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
            if (!IsInLayerMask(other.gameObject, HitLayer)) return;

            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(Damage, StateType);
            }
        }
        
        private bool IsInLayerMask(GameObject obj, LayerMask mask)
        {
            // Convert the object's layer (0–31) into a bitmask.
            // Example: layer 8 →
            // 1 << 8 →
            // 0001 0000 0000
            int objectLayerBit = 1 << obj.layer;
            
            // If the result is NOT zero ->
            // (this is zero) 0000 0000 0000
            // it means the mask contains this layer.
            return (mask.value & objectLayerBit) != 0;
        }
        
    }
}