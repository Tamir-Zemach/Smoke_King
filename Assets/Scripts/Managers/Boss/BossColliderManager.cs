using UnityEngine;

namespace Managers.Boss
{
    public class BossColliderManager : MonoBehaviour
    {
        [SerializeField] private CapsuleCollider2D _collider2D;
        public void EnableCol()
        {
            _collider2D.enabled = true;
        }

        public void UnAbleCol()
        {
            _collider2D.enabled = false;
        }
    }
}