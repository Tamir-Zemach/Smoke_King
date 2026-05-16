using UnityEngine;

namespace Player
{
    public class PlayerActionAnimation : MonoBehaviour
    {
        private PlayerParticleManager _particle;

        private void Awake()
        {
            _particle = GetComponentInParent<PlayerParticleManager>();
        }

        // Animation Event: Horizontal attack particle
        public void PlayHorizontalParticle()
        {
            _particle.PlayHorAttackPar();
        }

        // Animation Event: Vertical attack particle
        public void PlayVerticalParticle()
        {
            _particle.PlayVerAttackPar();
        }
    }
}
