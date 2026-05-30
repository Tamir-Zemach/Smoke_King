using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerAnimationManager : PlayerInputBehavior
    {
        private Animator _anim;
        private PlayerMovementManager _movement;
        private PlayerAttackManager _attack;
        private float _punchLayerVelocity;
        private bool _frozen = false;
        private Coroutine _freezeRoutine;


        protected override void Awake()
        {
            base.Awake();
            _anim = GetComponentInChildren<Animator>();
            _attack = GetComponentInChildren<PlayerAttackManager>();
            _movement = GetComponent<PlayerMovementManager>();
        }

        protected override void SubscribeToInputEvents()
        {
            Input.OnJump += HandleJump;
        }

        protected override void UnSubscribeToInputEvents()
        {
            Input.OnJump -= HandleJump;
        }

        private void Update()
        {
            if (_frozen)
                return;

            UpdateMovementAnimations();
            UpdateGroundedState();
            UpdateAttackState();
            UpdatePunchLayerWeight();
        }

        private void UpdatePunchLayerWeight()
        {
            bool punching = _attack.IsAttacking || _attack.IsAttackingUp;
            float targetWeight = punching ? 1f : 0f;

            float newWeight = Mathf.SmoothDamp(
                _anim.GetLayerWeight(1),
                targetWeight,
                ref _punchLayerVelocity,
                0.1f
            );

            _anim.SetLayerWeight(1, newWeight);
        }

        private void UpdateAttackState()
        {
            _anim.SetBool("IsAttacking", _attack.IsAttacking);
            _anim.SetBool("IsAttackingUp", _attack.IsAttackingUp);
        }

        private void UpdateMovementAnimations()
        {
            float speed = Mathf.Abs(Input.Movement.x);
            _anim.SetFloat("Speed", speed);
        }

        private void UpdateGroundedState()
        {
            if (_movement != null)
            {
                _anim.SetBool("IsGrounded", _movement.IsGrounded);
                _anim.SetBool("IsOnWall", _movement.IsOnWall);
            }
        }

        private void HandleJump()
        {
            _anim.SetTrigger("IsJumping");
        }

        // -----------------------------
        // FREEZE / UNFREEZE
        // -----------------------------
        public void FreezeAnimations(float delay = 0.05f)
        {
            if (_freezeRoutine != null)
            {
                StopCoroutine(_freezeRoutine);
            }

            _freezeRoutine = StartCoroutine(FreezeAfterDelay(delay));
        }

        private IEnumerator FreezeAfterDelay(float delay)
        {
            float t = 0f;
            while (t < delay)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            _frozen = true;
            _anim.speed = 0f;
        }

        public void UnfreezeAnimations()
        {
            if (_freezeRoutine != null)
                StopCoroutine(_freezeRoutine);

            _frozen = false;
            _anim.speed = 1f;
        }
        
        
        public void FreezeAnimationsImmediate()
        {
            if (_freezeRoutine != null)
                StopCoroutine(_freezeRoutine);

            _frozen = true;
            _anim.speed = 0f;
        }

    }
}
