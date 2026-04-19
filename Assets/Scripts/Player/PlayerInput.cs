using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Player
{
    public class PlayerInput : SingletonMonoBehaviour<PlayerInput>
    {
        public Action OnJump;
        public Action OnStateSwitch;
        public Action OnAttack;
        public Action OnUpAttack;
        public Action OnPause;

        public InputActionAsset PlayerInputAsset;
        
        private InputActionMap _playerActionMap;
        private InputActionMap _uiActionMap;
        
        private InputAction _attackAction;
        private InputAction _jumpAction;
        private InputAction _moveAction;
        private InputAction _stateSwitchAction;
        private InputAction _pauseAction;
        public Vector2 Movement { get; private set; }
        public bool FacingRight { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            if (PlayerInputAsset == null)
            {
                Debug.LogError("PlayerInputAsset is null");
                return;
            }
            _playerActionMap = PlayerInputAsset.FindActionMap("Player");
            _uiActionMap = PlayerInputAsset.FindActionMap("UI");

            _moveAction = PlayerInputAsset.FindAction("Move");
            _jumpAction = PlayerInputAsset.FindAction("Jump");
            _stateSwitchAction = PlayerInputAsset.FindAction("StateSwitch");
            _attackAction = PlayerInputAsset.FindAction("Attack");
            _pauseAction = PlayerInputAsset.FindAction("Pause");
            FacingRight = true;
        }

        private void Update()
        {
            Movement = _moveAction.ReadValue<Vector2>();
            CheckForLookSide();
            CheckForPressedButtons();
        }

        private void CheckForPressedButtons()
        {
            if (_pauseAction.WasPressedThisFrame())
            {
                OnPause?.Invoke();
            }
            if (_jumpAction.WasPressedThisFrame()) OnJump?.Invoke();
            if (_stateSwitchAction.WasPressedThisFrame()) OnStateSwitch?.Invoke();
            if (_attackAction.WasPressedThisFrame())
            {
                // Check if the player is pressing UP at the same time
                if (Movement.y > 0.5f)
                    OnUpAttack?.Invoke();
                else
                    OnAttack?.Invoke();
            }
        }

        private void CheckForLookSide()
        {
            switch (Movement.x)
            {
                case > 0:
                    FacingRight = true;
                    break;
                case < 0:
                    FacingRight = false;
                    break;
            }
        }
        
        public void ForceFacingDirection(bool faceRight)
        {
            FacingRight = faceRight;
        }

    }
}