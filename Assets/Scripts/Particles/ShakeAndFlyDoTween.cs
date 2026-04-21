using DG.Tweening;
using UnityEngine;

namespace Particles
{
    [ExecuteInEditMode]
    public class ShakeAndFlyDoTween : MonoBehaviour
    {
        [Header("Settings")]
        public float ShakeDuration = 0.3f;
        public float ShakeStrength = 0.2f;
        public int ShakeVibrato = 20;

        public float FlyDistance = 5f;
        public float FlyDuration = 0.5f;

        private Vector3 _originalPos;

        private void Awake()
        {
            _originalPos = transform.position;
        }
        public void PlayEffect()
        {
            Vector3 startPos = transform.position;

            Sequence seq = DOTween.Sequence();

            seq.Append(transform.DOShakePosition(
                ShakeDuration,
                ShakeStrength,
                ShakeVibrato,
                randomness: 90,
                snapping: false,
                fadeOut: true
            ));

            Vector3 targetPos = startPos + transform.up * FlyDistance;

            seq.Append(transform.DOMove(targetPos, FlyDuration).SetEase(Ease.OutQuad));
        }

        public void ResetPosition()
        {
            transform.position = _originalPos;
        }


    }
}