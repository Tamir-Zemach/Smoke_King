using System;
using Enums;
using ObjectPooling;
using UnityEngine;
using Utilities;

namespace Managers.Boss
{
    public class BossAnimManager : MonoBehaviour
    {
        [Header("Speed Control")]
        [Range(0.1f, 3f)]
        public float TimeMultiplier = 1f;

        private Animator _animator;

        public Action<BossParticles> OnParticlesPlay;
        public Action OnSpawnAnimEnds;
        public Action<BossAttacksTypes> OnStartAttack;
        public Action OnTeleportAnimEnds;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            ApplyTimeMultiplier();
        }

        private void OnValidate()
        {
            // Update in editor when value changes
            if (_animator == null)
                _animator = GetComponent<Animator>();

            ApplyTimeMultiplier();
        }

        public void ApplyTimeMultiplier()
        {
            if (_animator != null)
                _animator.speed = TimeMultiplier;
        }

        #region Trigger Functions

        public void TriggerAttackAnimation(BossAttacksTypes attackType)
        {
            var triggerName = attackType.ToString();

            if (!StringChecker.AnimatorHasString(_animator, triggerName)) return;

            _animator.SetTrigger(triggerName);
        }

        public void TriggerIntroAnimation()
        {
            _animator.SetTrigger("intro");
        }

        public void TriggerTeleportAnimation()
        {
            const string triggerName = "Teleport";

            if (!StringChecker.AnimatorHasString(_animator, triggerName)) return;

            _animator.SetTrigger(triggerName);
        }

        public void TriggerSpawnAnimation()
        {
            const string triggerName = "Spawn";

            if (!StringChecker.AnimatorHasString(_animator, triggerName)) return;

            _animator.SetTrigger(triggerName);
        }

        #endregion

        #region Animation Events

        public void StartAttack(BossAttacksTypes attackType)
        {
            OnStartAttack?.Invoke(attackType);
        }

        public void PlayParticles(BossParticles particles)
        {
            OnParticlesPlay?.Invoke(particles);
        }

        public void FinishTeleportAnimation()
        {
            BossParticlePool.Instance.Stop(BossParticles.KingFloatSmoke);
            OnTeleportAnimEnds?.Invoke();
        }

        public void FinishSpawnAnimation()
        {
            OnSpawnAnimEnds?.Invoke();
        }

        #endregion
    }
}
