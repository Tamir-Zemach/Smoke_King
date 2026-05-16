using Data;
using Enums;
using UnityEngine;

namespace Player
{
    public class PlayerParticleManager : MonoBehaviour
    {
        [SerializeField] private PlayerData _playerData;
        [SerializeField] private ParticleSystem _horAttackParSystem;
        [SerializeField] private ParticleSystem _verticalAttackParSystem;
        private PlayerStateManager _stateManager;

        private ParticleSystemRenderer _horAttackRenderer;
        private ParticleSystemRenderer _verticalAttackRenderer;

        private void Awake()
        {
            _horAttackRenderer = _horAttackParSystem.GetComponent<ParticleSystemRenderer>();
            _verticalAttackRenderer = _verticalAttackParSystem.GetComponent<ParticleSystemRenderer>();
            _stateManager = GetComponent<PlayerStateManager>();
        }

        public void PlayHorAttackPar()
        {
            ApplyMaterial(_horAttackRenderer);
            _horAttackParSystem.Play();
        }

        public void PlayVerAttackPar()
        {
            ApplyMaterial(_verticalAttackRenderer);
            _verticalAttackParSystem.Play();
        }

        private void ApplyMaterial(ParticleSystemRenderer particleSystemRenderer)
        {
            var visual = _playerData.GetVisual(_stateManager.CurrentStateType);

            // Apply material
            if (visual.Material != null)
            {
                particleSystemRenderer.material = visual.Material;
            }

            // Apply color
            //var main = particleSystemRenderer.GetComponent<ParticleSystem>().main;
           // main.startColor = visual.Color;
        }
    }
}