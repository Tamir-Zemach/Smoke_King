using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Player
{
    public class PlayerInput : SingletonMonoBehaviour<PlayerInput>
    {
        public Tutorial.PlayerInputBlocker TutorialBlocker { get; private set; }
        public Action OnJump;
        public Action OnStateSwitch;
        public Action OnAttack;
        public Action OnUpAttack;
        public Action OnPause;
        public event Action<Vector2> OnMovePerformed;
        public event Action OnMoveCanceled;

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
            TutorialBlocker = new Tutorial.PlayerInputBlocker(this);
            _moveAction.performed += ctx => OnMovePressed(ctx.ReadValue<Vector2>());
            _moveAction.canceled  += ctx => OnMoveReleased(); 

        }

        private void Update()
        {
            if (GameManager.Instance.GameIsPaused) return; // <--- STOP INPUT WHEN PAUSED

            Movement = _moveAction.ReadValue<Vector2>();
            CheckForLookSide();
            CheckForPressedButtons();
        }


        private void CheckForPressedButtons()
        {
            if (_pauseAction.WasPressedThisFrame())
                OnPause?.Invoke();

            if (_jumpAction.WasPressedThisFrame() && TutorialBlocker.CanJump())
                OnJump?.Invoke();

            if (_stateSwitchAction.WasPressedThisFrame() && TutorialBlocker.CanStateSwitch())
                OnStateSwitch?.Invoke();

            if (_attackAction.WasPressedThisFrame())
            {
                if (!TutorialBlocker.CanAttack()) return;

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
        private void OnMovePressed(Vector2 v)
        {
            OnMovePerformed?.Invoke(v);
        }

        private void OnMoveReleased()
        {
            OnMoveCanceled?.Invoke();
        }


    }
}