using Enums;
using UnityEngine;

namespace Interfaces
{
    public interface IDamageable
    {
        int maxHealth { get; set; }
        void TakeDamage(int damage, StateType stateType);
        public void TakeDamage(int damage, StateType stateType, Vector3 hitPoint);

        void AddHealth(int amount);

        bool IsInvincible();
        bool IsSameState(StateType stateType);
    }

}