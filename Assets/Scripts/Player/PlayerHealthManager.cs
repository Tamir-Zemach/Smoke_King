using System;
using System.Collections;
using Audio;
using Core;
using Data;
using Enums;
using Interfaces;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Player
{
    public class  PlayerHealthManager : HealthBase, IDamageable, IInvincible
    {
        public bool _1life = false;
        [SerializeField] private PlayerData _playerData;
        [SerializeField] private float _sameStateCooldown = 0.2f;


        public UnityEvent OnInvisible;
        public UnityEvent OnNormal;

        public float InvisibilityTime = 1f;

        private PlayerStateManager _playerStateManager;
        private bool _sameStateCooldownActive = false;
        public Action OnDying;
        public Action OnGettingDamage;
        public Action OnSameState;
        public bool IsInDeathSequence;


        protected override void Awake()
        {
            base.Awake();

            if (_playerData == null)
                Debug.LogWarning("PlayerData is missing on " + gameObject.name);

            _maxHealth = _playerData.MaxHealth;
            _currentHealth = _1life ? 1 : _maxHealth;
            _playerStateManager = GetComponent<PlayerStateManager>();
            
        }

        public int maxHealth { get; set; }

        // ORIGINAL signature (kept for compatibility)
        public void TakeDamage(int damage, StateType stateType)
        {
            // Fallback: no hit point provided → use player center
            TakeDamage(damage, stateType, transform.position);
        }

        public void TakeDamage(int damage, StateType stateType, Vector3 hitPoint)
        {
            if (_playerStateManager.CurrentStateType == stateType)
            {
                if (!_sameStateCooldownActive)
                {
                    _sameStateCooldownActive = true;

                    AudioManager.Instance.PlaySfx(SfxType.ParticleHittingPlayer, canOverlap: false);
                    OnSameState?.Invoke();

                    StartCoroutine(ResetSameStateCooldown());
                }

                return;
            }

            if (IsInvincible || IsInDeathSequence) return;

            SubtractHealth(damage);
            OnGettingDamage?.Invoke();
            AudioManager.Instance.PlaySfx(SfxType.PlayerGettingHurt);

            StartCoroutine(HealthUtils.Invisibility(this, InvisibilityTime));
        }
        
        private IEnumerator ResetSameStateCooldown()
        {
            yield return new WaitForSeconds(_sameStateCooldown);
            _sameStateCooldownActive = false;
        }


        bool IDamageable.IsInvincible() => IsInvincible;

        public bool IsSameState(StateType stateType)
        {
            return _playerStateManager.CurrentStateType == stateType;
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
