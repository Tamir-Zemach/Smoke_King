using Enums;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class PlayerStateManager : MonoBehaviour
    {
        public StateType CurrentStateType = StateType.State1;

        //sprite change logic for now 
        public Sprite SpriteForState1;
        public Sprite SpriteForState2;

        //For particals 
        public UnityEvent OnStateChange;
        private PlayerInput _playerInput;
        private SpriteRenderer _spriteRenderer;


        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _playerInput.OnStateSwitch += OnStateSwitch;
        }

        private void Update()
        {
        }

        private void OnStateSwitch()
        {
            switch (CurrentStateType)
            {
                case StateType.State1:
                    CurrentStateType = StateType.State2;
                    _spriteRenderer.sprite = SpriteForState2;
                    _spriteRenderer.color = Color.blue;
                    break;
                case StateType.State2:
                default:
                    CurrentStateType = StateType.State1;
                    _spriteRenderer.sprite = SpriteForState1;
                    _spriteRenderer.color = Color.red;
                    break;
            }
        }
    }
}