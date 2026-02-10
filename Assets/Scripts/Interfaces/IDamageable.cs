using Enums;

namespace Interfaces
{
    public interface IDamageable
    {
        int maxHealth { get; set; }
        void TakeDamage(int damage, StateType stateType);
        void AddHealth(int amount);
    }
}