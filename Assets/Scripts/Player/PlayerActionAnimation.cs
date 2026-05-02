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

        public void EnableHitbox()
        {
            _attack.EnableHitbox();
        }
    }
}