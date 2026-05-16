using System;
using Enums;
using UnityEngine;
using Utilities;

namespace Player
{
    public class PlayerStateManager : MonoBehaviour
    {
        public StateType CurrentStateType = StateType.State1;
        public Material PlayerMaterial;

        public Action OnStateChange;
        private PlayerInput _playerInput;
        private SpriteRenderer _spriteRenderer;


        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.OnStateSwitch += OnStateSwitch;
            CurrentStateType = StateType.State1;
            ShaderLerpUtility.LerpFloat(PlayerMaterial, "_Polarity", 0, 0.001f);
        }
        

        private void OnStateSwitch()
        {
            float targetPolarity;

            switch (CurrentStateType)
            {
                case StateType.State1:
                    CurrentStateType = StateType.State2;
                    targetPolarity = 1f;
                    break;

                default:
                    CurrentStateType = StateType.State1;
                    targetPolarity = 0f;
                    break;
            }

            OnStateChange?.Invoke();
            ShaderLerpUtility.LerpFloat(PlayerMaterial, "_Polarity", targetPolarity, 0.3f);
        }

    }
}