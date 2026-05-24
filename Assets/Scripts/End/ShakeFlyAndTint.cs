using DG.Tweening;
using UnityEngine;

namespace End
{
    public class ShakeFlyAndTint : MonoBehaviour
    {
        public float Duration = 0.5f;
        public float ShakeStrength = 0.5f;
        public int ShakeVibrato = 20;

        public float FlyUpDistance = 1f;
        private SpriteRenderer[] _spriteRenderers;

        void Awake()
        {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            // Clone materials so each sprite has its own instance
            foreach (var sr in _spriteRenderers)
                sr.material = new Material(sr.material);
        }

        public void PlayEffect()
        {
            transform.DOShakePosition(Duration, ShakeStrength, ShakeVibrato);

            transform.DOMoveY(transform.position.y + FlyUpDistance, Duration)
                .SetEase(Ease.OutQuad);

            foreach (var sr in _spriteRenderers)
            {
                sr.material.DOFloat(1f, "_WhiteAmount", Duration);
            }
        }
    }
}