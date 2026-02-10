using System;
using UnityEngine;
using Utilities;

namespace Core
{
    public abstract class HealthBase : MonoBehaviour
    {
        protected int _currentHealth;
        protected int _maxHealth;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;

        protected virtual void Awake()
        {
            _currentHealth = 1;
            _maxHealth = 1;
        }

        public event Action OnHealthChanged;

        public virtual void AddHealth(int amount)
        {
            _currentHealth += Math.Max(0, amount);
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);

            OnHealthChanged?.Invoke();
        }

        protected virtual void SubtractHealth(int amount)
        {
            _currentHealth -= Math.Max(0, amount);

            OnHealthChanged?.Invoke();

            if (_currentHealth <= 0) HandleDeath();
        }

        public virtual void IncreaseMaxHealth(int amount)
        {
            _maxHealth += Math.Max(0, amount);
            OnHealthChanged?.Invoke();
        }

        public virtual void SetMaxHealth(int amount)
        {
            _maxHealth = amount;
            OnHealthChanged?.Invoke();
        }

        public virtual void FullHealth()
        {
            _currentHealth = _maxHealth;
            OnHealthChanged?.Invoke();
        }

        public void DisplayHealthInConsole(string label = "Health")
        {
            HealthUtils.DisplayHealthInConsole(_currentHealth, _maxHealth, label);
        }

        // Every subclass MUST define what happens when it dies
        protected abstract void HandleDeath();
    }
}