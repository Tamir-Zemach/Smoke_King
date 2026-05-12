using System.Collections;
using Data;
using UnityEngine;

namespace Player
{
    public class PlayerAttackManager : PlayerInputBehavior
    {
        [Header("Data")]
        [SerializeField] private PlayerData _playerData;

        [Header("Hitbox")]
        [SerializeField] private BoxCollider2D _attackCollider;
        [SerializeField] private Transform _frontTransform;
        [SerializeField] private Transform _upTransform;

        private bool _isAttacking;
        private bool _isAttackingUp;
        private PlayerMovementManager _movement;

        public bool IsAttacking => _isAttacking;
        public bool IsAttackingUp => _isAttackingUp;

        protected override void Awake()
        {
            base.Awake();
            _movement = GetComponent<PlayerMovementManager>();
        }

        protected override void SubscribeToInputEvents()
        {
            Input.OnAttack += Attack;
            Input.OnUpAttack += UpAttack;
        }

        protected override void UnSubscribeToInputEvents()
        {
            Input.OnAttack -= Attack;
            Input.OnUpAttack -= UpAttack;
        }

        private void Attack() => TryStartAttack(false);
        private void UpAttack() => TryStartAttack(true);

        private void TryStartAttack(bool attackingUp)
        {
            if (_isAttacking || _isAttackingUp || _movement.IsOnWall) return;
            _attackCollider.gameObject.SetActive(false);
            _isAttacking = false;
            _isAttackingUp = false;
            PositionAttackCollider(attackingUp);
            StartCoroutine(AttackFailsafe());
        }

        private void PositionAttackCollider(bool attackingUp)
        {
            if (attackingUp)
            {
                _isAttackingUp = true;
                _isAttacking = false;
                _attackCollider.transform.position = _upTransform.position;
            }
            else
            {
                _isAttacking = true;
                _isAttackingUp = false;
                _attackCollider.transform.position = _frontTransform.position;
            }
        }

        private IEnumerator AttackRoutine()
        {
            // Wait for the attack window
            yield return new WaitForSeconds(_playerData.AttackDuration);
            
            _isAttacking = false;
            _isAttackingUp = false;
            _attackCollider.gameObject.SetActive(false);
        }
        private IEnumerator AttackFailsafe()
        {
            yield return new WaitForSeconds(_playerData.AttackDuration * 1.5f);

            if (!_isAttacking && !_isAttackingUp) yield break;
            _isAttacking = false;
            _isAttackingUp = false;
            _attackCollider.gameObject.SetActive(false);
        }


        // -----------------------------
        // CALLED BY ANIMATION EVENTS
        // -----------------------------
        public void EnableHitbox()
        {
            _attackCollider.gameObject.SetActive(true);
            StartCoroutine(AttackRoutine());
        }
        
    }
}
