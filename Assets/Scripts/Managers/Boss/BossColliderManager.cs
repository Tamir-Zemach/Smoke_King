using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Managers.Boss
{
    public class BossColliderManager : MonoBehaviour
    {
        [SerializeField] private CapsuleCollider2D _collider2D;

        // Automatically filled with all ShadowCaster2D components in children
        private ShadowCaster2D[] _shadowCasters;

        private void Awake()
        {
            // Get all ShadowCaster2D components in this object’s children (including inactive)
            _shadowCasters = GetComponentsInChildren<ShadowCaster2D>(true);
        }

        public void EnableCol()
        {
            _collider2D.enabled = true;

            if (_shadowCasters == null) return;
            foreach (var caster in _shadowCasters)
            {
                caster.enabled = true;
            }


        }

        public void UnAbleCol()
        {
            _collider2D.enabled = false;

            if (_shadowCasters == null) return;
            foreach (var caster in _shadowCasters)
            {
                caster.enabled = false;
            }
        }
    }
}