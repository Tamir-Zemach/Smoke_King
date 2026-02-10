using Core;
using Interfaces;
using UnityEngine;

namespace Player
{
    public abstract class PlayerInputBehavior : MonoBehaviour, IInputGetter
    {
        public PlayerInput Input { get; set; }
        
        protected virtual void Awake()
        {
            Input = GetComponent<PlayerInput>();
        }

        protected virtual void Start()
        {
            SubscribeToInputEvents();
        }

        private void OnDestroy()
        {
            UnSubscribeToInputEvents();
        }

        protected abstract void SubscribeToInputEvents();
        protected abstract void UnSubscribeToInputEvents();
    }
}