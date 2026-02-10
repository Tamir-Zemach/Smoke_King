
    using System;
    using Core;
    using Interfaces;
    using Enums;
    using UnityEngine;
    using UnityEngine.Events;
    using Utilities;
    
namespace Player
{
        public class PlayerHealthManager : PlayerHealthBase, IDamageable, IInvincible
        {
            public Action OnGettingDamage;
            public UnityEvent OnInvisible;
            public UnityEvent OnNormal;

            private PlayerStateManager _playerStateManager;

            public bool IsInvincible { get; set; }
            public int maxHealth { get; set; }

            public float InvisibilityTime = 1f;

            protected override void Awake()
            {
                base.Awake();
                _playerStateManager = GetComponent<PlayerStateManager>();
            }

            public void TakeDamage(int damage, StateType stateType)
            {
                if (IsInvincible) return;
                if (_playerStateManager.CurrentStateType == stateType) return;

                SubtractHealth(damage);
                OnGettingDamage?.Invoke();
                StartCoroutine(HealthUtils.Invisibility(this, InvisibilityTime));
            }

            protected override void HandleDeath()
            {
                Debug.Log("Player died!");
            }

            public void OnInvincibleStart() => OnInvisible?.Invoke();
            public void OnInvincibleEnd() => OnNormal?.Invoke();
        }
    
}