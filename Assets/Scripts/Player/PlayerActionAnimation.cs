using UnityEngine;

namespace Player
{
    public class PlayerActionAnimation : MonoBehaviour
    {
        private PlayerAttackManager _attack;
        private PlayerParticleManager _particle;

        private void Awake()
        {
            _attack = GetComponentInParent<PlayerAttackManager>();
            _particle = GetComponentInParent<PlayerParticleManager>();
        }

        // Animation Event: Horizontal attack
        public void EnableHorizontalHitbox()
        {
            _particle.PlayHorAttackPar();
            _attack.EnableHitbox();
        }

        // Animation Event: Vertical attack
        public void EnableVerticalHitbox()
        {
            _particle.PlayVerAttackPar();
            _attack.EnableHitbox();
        }
    }
}