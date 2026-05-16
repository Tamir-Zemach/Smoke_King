using System.Collections;
using Data;
using UnityEngine;

namespace Player
{
    public class PlayerAttackManager : PlayerInputBehavior
    {
        [Header("Data")]
        [SerializeField] private PlayerData _playerData;
        [SerializeField] private PlayerParticleManager _particle;

        [Header("Hitbox")]
        [SerializeField] private BoxCollider2D _attackCollider;
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
            // Prevent spam or attacking on wall
            if (_isAttacking || _isAttackingUp || _movement.IsOnWall)
            {
                Debug.Log("Cant Attack");
                return;
            }
            
            _isAttackingUp = up;
            _isAttacking   = !up;

            // Position hitbox
            _attackCollider.transform.position = up
                ? _upTransform.position
                : _frontTransform.position;

            StartCoroutine(AttackRoutine());
        }

        private IEnumerator AttackRoutine()
        {
            // Delay before hitbox + particles (sync with animation)
            yield return new WaitForSeconds(_playerData.DelayBeforeHitBox);

            // Play particles
            PlayParticle(_isAttacking);

            // Enable hitbox
            _attackCollider.gameObject.SetActive(true);

            // Attack lasts exactly AttackDuration
            yield return new WaitForSeconds(_playerData.AttackDuration - _playerData.DelayBeforeHitBox);

            // End attack
            _isAttacking = false;
            _isAttackingUp = false;
            _attackCollider.gameObject.SetActive(false);
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
