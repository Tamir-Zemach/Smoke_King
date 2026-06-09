using System;
using Ui;
using UnityEngine;

namespace Player
{
    public class PlayerFlashLightController : MonoBehaviour
    {
        [SerializeField] private PlayerHealthManager _playerHealthManager;
        [SerializeField] private Light2DPulse _light2DPulse;


        private void Awake()
        {
            _playerHealthManager.OnSameState += FlashLight;
        }

        private void OnDestroy()
        {
            _playerHealthManager.OnSameState -= FlashLight;
        }

        private void FlashLight()
        {
            _light2DPulse.Pulse();
        }
    }
}
