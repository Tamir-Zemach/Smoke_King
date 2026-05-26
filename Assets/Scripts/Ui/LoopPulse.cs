using DG.Tweening;
using UnityEngine;

namespace Ui
{
    public class LoopPulse : MonoBehaviour
    {
        [Header("Pulse Settings")]
        [SerializeField] private float _pulseScale = 1.3f;
        [SerializeField] private float _pulseTime = 0.15f;
        [SerializeField] private float _returnTime = 0.15f;
        [SerializeField] private Ease _pulseEase = Ease.InOutSine;

        private Tween _pulseTween;
        private float _defaultScale;

        private void OnEnable()
        {
            _defaultScale = transform.localScale.x;
            StartPulse();
        }

        private void OnDisable()
        {
            _pulseTween?.Kill();
        }

        public void StartPulse()
        {
            _pulseTween?.Kill();

            Sequence seq = DOTween.Sequence();

            // Pulse up relative to default scale
            seq.Append(
                UiMovementUtility.Pulse(
                    transform,
                    _pulseScale,
                    _pulseTime
                )
            );

            // Return to default scale
            seq.Append(
                transform
                    .DOScale(_defaultScale, _returnTime)
                    .SetEase(_pulseEase)
            );

            seq.SetLoops(-1, LoopType.Restart);

            _pulseTween = seq;
        }
    }
}