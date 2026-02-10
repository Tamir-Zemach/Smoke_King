
using System.Collections;
using Data;
using UnityEngine;

namespace Player
{
    public class PlayerAttackManager : PlayerInputBehavior
    {
        [SerializeField] private PlayerData _playerData;
        
        [SerializeField] private BoxCollider2D _attackCollider;
        [SerializeField] private Transform _rightSideTransform;
        [SerializeField] private Transform _leftSideTransform;
        [SerializeField] private Transform _upTransform;

        private bool _isAttacking;
        
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


        private void Attack()
        {
            TryStartAttack(false);
        }

        private void UpAttack()
        {
            TryStartAttack(true);
        }

        private void TryStartAttack(bool attackingUp)
        {
            if (_isAttacking) return;
            StartCoroutine(AttackRoutine(attackingUp));
        }

        private IEnumerator AttackRoutine(bool attackingUp)
        {
            _isAttacking = true;

            PositionAttackCollider(attackingUp);
            _attackCollider.gameObject.SetActive(true);

            yield return new WaitForSeconds(_playerData.AttackDuration);

            _attackCollider.gameObject.SetActive(false);

            yield return new WaitForSeconds(_playerData.AttackBufferTime);

            _isAttacking = false;
        }

        private void PositionAttackCollider(bool attackingUp)
        {
            if (attackingUp)
            {
                _attackCollider.transform.position = _upTransform.position;
                return;
            }

            _attackCollider.transform.position =
                Input.FacingRight ? _rightSideTransform.position : _leftSideTransform.position;
        }


    }
}