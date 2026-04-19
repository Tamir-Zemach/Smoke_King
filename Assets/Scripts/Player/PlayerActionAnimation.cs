using UnityEngine;

namespace Player
{
    public class PlayerActionAnimation : MonoBehaviour
    {
        private PlayerAttackManager _attack;

        private void Awake()
        {
            _attack = GetComponentInParent<PlayerAttackManager>();
        }

        // Animation event: when the sword swing reaches the hit frame
        public void EnableHitbox()
        {
            _attack.EnableHitbox();
        }
    }
}