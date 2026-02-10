using System;
using Data;
using Interfaces;
using UnityEngine;

namespace Core
{
        public abstract class PlayerDataBehaviour : MonoBehaviour
        {
            [SerializeField] protected PlayerData _playerData;
            public PlayerData PlayerData
            {
                get => _playerData;
                set => _playerData = value;
            }
            
        }
}