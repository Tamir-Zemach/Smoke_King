using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : InputBehaviour
    {
        private CircleCollider2D _collider;

        // NEW — smooth control return
        private float _controlFactor = 1f; // 0 = no control, 1 = full control

        private bool _isGrounded;
        private bool _onWall;
        private bool _onWallLeft;
        private bool _onWallRight;
        private PlayerHealthManager _playerHealthManager;
        private Rigidbody2D _rb;


        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CircleCollider2D>();
            _playerHealthManager = GetComponent<PlayerHealthManager>();
        }


        // -----------------------------
        // UPDATE
        // -----------------------------
        private void Update()
        {
            CheckForGround();
            CheckForWall();
        }


        // -----------------------------
        // MOVEMENT
        // -----------------------------
        private void FixedUpdate()
        {
            if (_rb == null) return;

            // Movement uses easing
            var targetX = Input.Movement.x * _playerData.Speed;

            var easedX = Mathf.Lerp(_rb.linearVelocity.x, targetX, _controlFactor);

            _rb.linearVelocity = new Vector2(easedX, _rb.linearVelocity.y);
        }


        // -----------------------------
        // KNOCKBACK
        // -----------------------------
        private void ApplyKnockBack()
        {
            _controlFactor = 0f;

            var randomFactor = Random.Range(0, 3);
            var force = _playerData.KnockBackForce + randomFactor;

            var knockBackDir = Input.FacingRight
                ? new Vector2(-force, force)
                : new Vector2(force, force);

            _rb.AddForce(knockBackDir, ForceMode2D.Impulse);

            Invoke(nameof(GainControl), 0.25f);
        }

        private void GainControl()
        {
            StartCoroutine(EaseControlBack());
        }

        private IEnumerator EaseControlBack()
        {
            var t = 0f;

            while (t < _playerData.EaseControlBackDur)
            {
                t += Time.deltaTime;
                _controlFactor = t / _playerData.EaseControlBackDur;
                yield return null;
            }

            _controlFactor = 1f;
        }


        // -----------------------------
        // WALL & GROUND CHECKS
        // -----------------------------
        private void CheckForWall()
        {
            var distance = 1f;
            Vector2 pos = transform.position;

            _onWallLeft = Physics2D.Raycast(pos, Vector2.left, distance, _playerData.WallMask);
            _onWallRight = Physics2D.Raycast(pos, Vector2.right, distance, _playerData.WallMask);

            _onWall = _onWallLeft || _onWallRight;
        }

        private void CheckForGround()
        {
            var groundCol = Physics2D.OverlapCircle(transform.position, _collider.radius, _playerData.GroundMask);
            _isGrounded = groundCol != null;
        }


        // -----------------------------
        // JUMP & WALL JUMP
        // -----------------------------
        private void Jump()
        {
            // Normal jump
            if (_isGrounded)
            {
                _rb.AddForce(Vector2.up * _playerData.JumpForce, ForceMode2D.Impulse);
                return;
            }

            // Wall jump
            if (_onWall)
            {
                _controlFactor = 0f;

                var opposite = _onWallLeft ? 1 : -1;

                var forceDir = new Vector2(
                    opposite * _playerData.JumpForceFromWallX,
                    _playerData.JumpForceFromWallY
                );

                _rb.linearVelocity = Vector2.zero;
                _rb.AddForce(forceDir, ForceMode2D.Impulse);

                Invoke(nameof(GainControl), 0.25f);
            }
        }


        // -----------------------------
        // INPUT EVENTS
        // -----------------------------
        protected override void SubscribeToInputEvents()
        {
            Input.OnJump += Jump;
            _playerHealthManager.OnGettingDamage += ApplyKnockBack;
        }

        protected override void UnSubscribeToInputEvents()
        {
            Input.OnJump -= Jump;
            _playerHealthManager.OnGettingDamage -= ApplyKnockBack;
        }
    }
}