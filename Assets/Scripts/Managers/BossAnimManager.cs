using System;
using System.Collections;
using Boss;
using Enums;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class BossAnimManager: MonoBehaviour
    {
        public Action OnTeleportAnimEnds;
        public Action OnSpawnAnimEnds;
        public Action OnTeleportTimerEnd;
        public Action<BossAttacksTypes> OnStartAttack;
        public Action <BossParticles>OnParticlesPlay;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }


        #region Trigger Functions

        public void TriggerAttackAnimation(BossAttacksTypes attackType)
        {
            string triggerName = attackType.ToString();

            if (!AnimationStringChecker.AnimatorHasString(_animator, triggerName)) return;

            _animator.SetTrigger(triggerName);
        }
        
        public void TriggerIntroAnimation()
        {
            _animator.SetTrigger("intro");
        }

        public void TriggerTeleportAnimation()
        {
            string triggerName = "Teleport";
            
            if (!AnimationStringChecker.AnimatorHasString(_animator, triggerName)) return;
            
            _animator.SetTrigger(triggerName);
        }

        public void TriggerSpawnAnimation()
        {
            string triggerName = "Spawn";
            
            if (!AnimationStringChecker.AnimatorHasString(_animator, triggerName)) return;
            
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

        public void FinishSpanAnimation()
        {
            OnSpawnAnimEnds?.Invoke();
        }

        #endregion


        public void StartTimerToTeleport(float time)
        {
            StartCoroutine(Timer(time));
        }

        private IEnumerator Timer(float time)
        {
            yield return new WaitForSeconds(time);
            OnTeleportTimerEnd?.Invoke();
        }
    }
}