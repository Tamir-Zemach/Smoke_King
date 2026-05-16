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
        private bool _attackLocked;

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
            // Prevent spam or restarting the attack
            if (_attackLocked || _isAttacking || _isAttackingUp || _movement.IsOnWall)
                return;

            // Lock input briefly
            _attackLocked = true;
            StartCoroutine(AttackLockout());

            // Set attack state
            _isAttacking = !attackingUp;
            _isAttackingUp = attackingUp;

            // Prepare hitbox
            _attackCollider.gameObject.SetActive(false);
            _attackCollider.transform.position = attackingUp ?
                _upTransform.position :
                _frontTransform.position;

            // Failsafe in case animation event never fires
            StartCoroutine(AttackFailsafe());
        }

        // Called by animation event
        public void EnableHitbox()
        {
            if (!_isAttacking && !_isAttackingUp)
                return; // Attack was cancelled or interrupted

            _attackCollider.gameObject.SetActive(true);
            StartCoroutine(AttackRoutine());
        }

        private IEnumerator AttackRoutine()
        {
            yield return new WaitForSeconds(_playerData.AttackDuration);
            EndAttack();
        }

        private IEnumerator AttackFailsafe()
        {
            yield return new WaitForSeconds(_playerData.AttackDuration * 1.5f);

            // If animation event never fired, clean up
            if (_isAttacking || _isAttackingUp)
                EndAttack();
        }

        private IEnumerator AttackLockout()
        {
            yield return new WaitForSeconds(0.1f); // tweak for responsiveness
            _attackLocked = false;
        }

        private void EndAttack()
        {
            _isAttacking = false;
            _isAttackingUp = false;
            _attackCollider.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_attackCollider == null)
                return;

            // Forward attack gizmo
            if (_frontTransform != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(_frontTransform.position, _attackCollider.size);
            }

            // Up attack gizmo
            if (_upTransform != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(_upTransform.position, _attackCollider.size);
            }
        }
#endif
    }
}
