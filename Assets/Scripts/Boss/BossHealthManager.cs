using System;
using Core;
using Interfaces;
using Enums;
using UnityEngine;
using Utilities;

namespace Boss
{
    public class BossHealthManager : HealthBase, IDamageable , IInvincible
    {
        [SerializeField] private float InvisibilityTime = 1;
        public event Action OnBossDied;
        public Action OnBossHit;
        public int maxHealth { get; set; }
        public bool IsInvincible { get; set; }

        protected override void Awake()
        {
            base.Awake();
            _maxHealth = 10;
            _currentHealth = _maxHealth;
        }


        public void TakeDamage(int damage, StateType stateType)
        {
            if (IsInvincible) return;
            Debug.Log($"took {damage} damage, current health is {_currentHealth}");
            SubtractHealth(damage);
            OnBossHit?.Invoke();
            StartCoroutine(HealthUtils.Invisibility(this, InvisibilityTime));            
        }
        

        protected override void HandleDeath()
        {
            Debug.Log("Boss died!");
            OnBossDied?.Invoke();
        }

        public void OnInvincibleStart()
        {
            
        }

        public void OnInvincibleEnd()
        {
            
        }
    }
}