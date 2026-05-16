using UnityEngine;

namespace Player
{
    public class PlayerAnimationManager : PlayerInputBehavior
    {
        private Animator _anim;
        private PlayerMovementManager _movement;
        private PlayerAttackManager _attack;
        private float _punchLayerVelocity;


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
                _anim.GetLayerWeight(1),   // current weight
                targetWeight,              // target weight
                ref _punchLayerVelocity,   // smoothing velocity
                0.1f                      // smooth time (tweakable)
            );

            _anim.SetLayerWeight(1, newWeight);
        }



        private void UpdateAttackState()
        {
            _anim.SetBool("IsAttacking", _attack.IsAttacking);
            _anim.SetBool("IsAttackingUp", _attack.IsAttackingUp);
        }

        // -----------------------------
        // MOVEMENT
        // -----------------------------
        private void UpdateMovementAnimations()
        {
            float speed = Mathf.Abs(Input.Movement.x);
            _anim.SetFloat("Speed", speed);
            
        }

        // -----------------------------
        // GROUND CHECK
        // -----------------------------
        private void UpdateGroundedState()
        {
            if (_movement != null)
            {
                _anim.SetBool("IsGrounded", _movement.IsGrounded);
                _anim.SetBool("IsOnWall", _movement.IsOnWall);
            }
        }

        // -----------------------------
        // JUMP
        // -----------------------------
        private void HandleJump()
        {
            _anim.SetTrigger("IsJumping");
        }
        
    }
}
