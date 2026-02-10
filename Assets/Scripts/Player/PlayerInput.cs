using System;
using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        public event Action OnJump;
        public event Action OnStateSwitch;
        public event Action OnAttack;
        public event Action OnUpAttack;
    
        public InputActionAsset PlayerInputAsset;
    
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _stateSwitchAction;
        private InputAction _attackAction;
        public Vector2 Movement { get; private set; }
        public bool FacingRight { get; private set; }
        
        private void Awake()
        {
            if (PlayerInputAsset == null)
            {
                Debug.LogError("PlayerInputAsset is null");
                return;
            }
            _moveAction =  PlayerInputAsset.FindAction("Move");
            _jumpAction = PlayerInputAsset.FindAction("Jump");
            _stateSwitchAction = PlayerInputAsset.FindAction("StateSwitch");
            _attackAction = PlayerInputAsset.FindAction("Attack");
            FacingRight = true;
        }
    
        private void Update()
        {
            Movement =  _moveAction.ReadValue<Vector2>();
            CheckForLookSide();
            if (_jumpAction.WasPressedThisFrame())
            {
                OnJump?.Invoke();
            }

            if (_stateSwitchAction.WasPressedThisFrame())
            {
                OnStateSwitch?.Invoke();
            }

            if (_attackAction.WasPressedThisFrame())
            {
                // Check if the player is pressing UP at the same time
                if (Movement.y > 0.5f)
                {
                    OnUpAttack?.Invoke();
                }
                else
                {
                    OnAttack?.Invoke();
                }
            }
       
        }
        private void CheckForLookSide()
        {
            if (Movement.x > 0)
            {
                FacingRight = true;
            }
            else if (Movement.x < 0)
            {
                FacingRight = false;
            }
            
        }



    }
}
