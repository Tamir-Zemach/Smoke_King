using System;
using Core;
using Enums;
using Interfaces;
using UnityEngine;
using Utilities;

namespace Boss
{
    public class BossHealthManager : HealthBase, IDamageable, IInvincible
    {
        [SerializeField] private float _invisibilityTime = 1;
        public Action OnBossHit;

        protected override void Awake()
        {
            base.Awake();
            _maxHealth = 20;
            _currentHealth = _maxHealth;
        }

        public int maxHealth { get; set; }


        public void TakeDamage(int damage, StateType stateType)
        {
            if (IsInvincible) return;
            SubtractHealth(damage);
            OnBossHit?.Invoke();
            StartCoroutine(HealthUtils.Invisibility(this, _invisibilityTime));
        }

        bool IDamageable.IsInvincible()
        {
            return IsInvincible;
        }

        public bool IsSameState(StateType stateType)
        {
            return false;
        }


        public bool IsInvincible { get; set; }

        public void OnInvincibleStart()
        {
        }

        public void OnInvincibleEnd()
        {
        }

        public event Action OnBossDied;


        protected override void HandleDeath()
        {
            Debug.Log("Boss died!");
            OnBossDied?.Invoke();
        }
    }
}