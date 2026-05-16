using Data;
using Enums;
using UnityEngine;
using Particles;
using Post_Processing;
using UnityEngine.Rendering.Universal;

namespace Player
{
    public class PlayerParticleManager : MonoBehaviour
    {
        [SerializeField] private PlayerData _playerData;
        [SerializeField] private ParticleSystem _horAttackParSystem;
        [SerializeField] private ParticleSystem _verticalAttackParSystem;
        [SerializeField] private ParticleSystem _circleOnStateChangeParSystem;
        [SerializeField] private DiagonalMover _diagonalMover;

        private PlayerStateManager _stateManager;
        private ParticleSystemRenderer _horAttackRenderer;
        private ParticleSystemRenderer _verticalAttackRenderer;
        private ParticleSystemRenderer _circleOnStateChangeRenderer;

        private void Awake()
        {
            _horAttackRenderer = _horAttackParSystem.GetComponent<ParticleSystemRenderer>();
            _verticalAttackRenderer = _verticalAttackParSystem.GetComponent<ParticleSystemRenderer>();
            _circleOnStateChangeRenderer = _circleOnStateChangeParSystem.GetComponent<ParticleSystemRenderer>();

            _stateManager = GetComponent<PlayerStateManager>();

            // Subscribe to state change
            _stateManager.OnStateChange += OnPlayerStateChanged;
        }

        private void OnDestroy()
        {
            // Always unsubscribe
            if (_stateManager != null)
            {
                _stateManager.OnStateChange -= OnPlayerStateChanged;
            }
        }

        private void OnPlayerStateChanged()
        {
            // Trigger diagonal movement
            if (_diagonalMover == null && _circleOnStateChangeParSystem == null) return;
            _diagonalMover.Move();
            ApplyColor(_circleOnStateChangeRenderer);
            _circleOnStateChangeParSystem.Play();
        }

        private void ApplyColor(ParticleSystemRenderer particleSystemRenderer)
        {
            var visual = _playerData.GetVisual(_stateManager.CurrentStateType);

            // Get the root particle system
            var rootPs = particleSystemRenderer.GetComponent<ParticleSystem>();

            // Apply to root
            var main = rootPs.main;
            main.startColor = visual.Color;

            // Apply to all children
            var childParticles = rootPs.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var ps in childParticles)
            {
                var childMain = ps.main;
                childMain.startColor = visual.Color;
            }
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


        }
    }
}
