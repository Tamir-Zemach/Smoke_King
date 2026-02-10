using System;
using Core;
using Data;
using Interfaces;
using UnityEngine;

namespace Player
{
    public abstract class InputBehaviour : PlayerDataBehaviour
    {
        protected PlayerInput Input;

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