using System;
using System.Collections;
using Enums;
using UnityEngine;
using Utilities;

namespace Managers.Boss
{
    public class BossAnimManager : MonoBehaviour
    {
        private Animator _animator;
        public Action<BossParticles> OnParticlesPlay;
        public Action OnSpawnAnimEnds;
        public Action<BossAttacksTypes> OnStartAttack;
        public Action OnTeleportAnimEnds;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
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
            var triggerName = "Teleport";

            if (!StringChecker.AnimatorHasString(_animator, triggerName)) return;

            _animator.SetTrigger(triggerName);
        }

        public void TriggerSpawnAnimation()
        {
            var triggerName = "Spawn";

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
            OnTeleportAnimEnds?.Invoke();
        }

        public void FinishSpawnAnimation()
        {
            OnSpawnAnimEnds?.Invoke();
        }

        #endregion
    }
}