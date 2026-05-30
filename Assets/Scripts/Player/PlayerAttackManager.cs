using System.Collections;
using Data;
using UnityEngine;
using Utilities;

namespace Player
{
    public class PlayerAttackManager : PlayerInputBehavior
    {
        [Header("Data")]
        [SerializeField] private PlayerData _playerData;
        [SerializeField] private PlayerParticleManager _particle;
        [SerializeField] private PlayerStateManager _stateManager;

        [Header("Hitbox")]
        [SerializeField] private DamageGiver _attackCollider;
        [SerializeField] private Transform _frontTransform;
        [SerializeField] private Transform _upTransform;

        private PlayerMovementManager _movement;

        private bool _isAttacking;
        private bool _isAttackingUp;
        public bool IsAttacking   => _isAttacking;
        public bool IsAttackingUp => _isAttackingUp;

        protected override void Awake()
        {
            base.Awake();
            _movement = GetComponent<PlayerMovementManager>();
        }

        protected override void SubscribeToInputEvents()
        {
            Input.OnAttack   += Attack;
            Input.OnUpAttack += UpAttack;
        }

        protected override void UnSubscribeToInputEvents()
        {
            Input.OnAttack   -= Attack;
            Input.OnUpAttack -= UpAttack;
        }

        private void Attack()   => TryStartAttack(false);
        private void UpAttack() => TryStartAttack(true);

        private void TryStartAttack(bool up)
        {
            if (_isAttacking || _isAttackingUp || _movement.IsOnWall)
            {
                return;
            }

            _isAttackingUp = up;
            _isAttacking   = !up;

            _attackCollider.transform.position = up
                ? _upTransform.position
                : _frontTransform.position;

            StartCoroutine(AttackRoutine());
        }

        private IEnumerator AttackRoutine()
        {
            // 1. Wait until the swing reaches the hit frame
            yield return new WaitForSeconds(_playerData.DelayBeforeHitBox);

            // 2. Play particles
            PlayParticle(_isAttacking);

            // 3. Enable hitbox for a short window
            _attackCollider.StateType = _stateManager.CurrentStateType;
            _attackCollider.gameObject.SetActive(true);
            yield return new WaitForSeconds(_playerData.HitboxDuration);
            _attackCollider.gameObject.SetActive(false);

            // 4. Let the rest of the animation / attack state finish
            float remaining =
                _playerData.AttackDuration
                - _playerData.DelayBeforeHitBox
                - _playerData.HitboxDuration;

            if (remaining > 0f)
                yield return new WaitForSeconds(remaining);

            // 5. End attack state
            _isAttacking = false;
            _isAttackingUp = false;
        }

        private void PlayParticle(bool horizontal)
        {
            if (horizontal)
                _particle.PlayHorAttackPar();
            else
                _particle.PlayVerAttackPar();
        }
    }
}
