using System;
using Audio;
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
        [SerializeField] private int _bossHealth = 1;

        public Action<Vector3, StateType> OnBossHit;

        protected override void Awake()
        {
            base.Awake();
            _maxHealth = _bossHealth;
            _currentHealth = _maxHealth;
        }

        public int maxHealth { get; set; }

        // OLD signature — fallback
        public void TakeDamage(int damage, StateType stateType)
        {
            TakeDamage(damage, stateType, transform.position);
        }

        // NEW signature — with hit point
        public void TakeDamage(int damage, StateType stateType, Vector3 hitPoint)
        {
            if (IsInvincible) return;

            SubtractHealth(damage);
            OnBossHit?.Invoke(hitPoint, stateType);
            AudioManager.Instance.PlaySfx(SfxType.BossGettingHit);

            StartCoroutine(HealthUtils.Invisibility(this, _invisibilityTime));
        }

        bool IDamageable.IsInvincible() => IsInvincible;

        public bool IsSameState(StateType stateType) => false;

        public bool IsInvincible { get; set; }

        public void OnInvincibleStart() { }
        public void OnInvincibleEnd() { }

        public event Action OnBossDied;

        protected override void HandleDeath()
        {
            Debug.Log("Boss died!");
            OnBossDied?.Invoke();
        }
    }
}