using Enums;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Player
{
    public class PlayerStateManager : MonoBehaviour
    {
        public StateType CurrentStateType = StateType.State1;
        public Material PlayerMaterial;

        //For particals 
        public UnityEvent OnStateChange;
        private PlayerInput _playerInput;
        private SpriteRenderer _spriteRenderer;


        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.OnStateSwitch += OnStateSwitch;
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

            ShaderLerpUtility.LerpFloat(PlayerMaterial, "_Polarity", targetPolarity, 0.3f);
        }

    }
}