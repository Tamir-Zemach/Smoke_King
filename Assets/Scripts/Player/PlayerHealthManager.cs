using System;
using Core;
using Data;
using Enums;
using Interfaces;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Player
{
    public class PlayerHealthManager : HealthBase, IDamageable, IInvincible
    {
        [SerializeField] private PlayerData _playerData;

        public UnityEvent OnInvisible;
        public UnityEvent OnNormal;

        public float InvisibilityTime = 1f;

        private PlayerStateManager _playerStateManager;
        public Action OnDying;
        public Action OnGettingDamage;

        protected override void Awake()
        {
            base.Awake();

            if (_playerData == null) Debug.LogWarning("PlayerData is missing on " + gameObject.name);

            _maxHealth = _playerData.MaxHealth;
            _currentHealth = _maxHealth;
            _playerStateManager = GetComponent<PlayerStateManager>();
        }

        public int maxHealth { get; set; }

        public void TakeDamage(int damage, StateType stateType)
        {
            if (IsInvincible) return;
            if (_playerStateManager.CurrentStateType == stateType) return;

            SubtractHealth(damage);
            OnGettingDamage?.Invoke();
            StartCoroutine(HealthUtils.Invisibility(this, InvisibilityTime));
        }

        public bool IsInvincible { get; set; }

        public void OnInvincibleStart()
        {
            OnInvisible?.Invoke();
        }

        public void OnInvincibleEnd()
        {
            OnNormal?.Invoke();
        }

        protected override void HandleDeath()
        {
            OnDying?.Invoke();
        }
    }
}