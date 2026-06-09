using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Ui
{
    public class Light2DPulse : MonoBehaviour
    {
        [Header("Pulse Settings")]
        [SerializeField] private float _pulseIntensity = 1.3f;
        [SerializeField] private float _pulseTime = 0.15f;
        [SerializeField] private float _returnTime = 0.15f;
        [SerializeField] private Ease _ease = Ease.InOutSine;

        private Light2D _light;
        private Tween _pulseTween;
        private float _defaultIntensity;

        private void Awake()
        {
            _light = GetComponent<Light2D>();
        }

        private void OnEnable()
        {
            if (_light == null)
                _light = GetComponent<Light2D>();

            _defaultIntensity = _light.intensity;
        }

        private void OnDisable()
        {
            _pulseTween?.Kill();
        }

        /// <summary>
        /// Pulses the light intensity once, then returns to default.
        /// </summary>
        public void Pulse()
        {
            if (_light == null)
                return;
            _pulseTween?.Kill();

            Sequence seq = DOTween.Sequence();

            // Pulse up
            seq.Append(
                DOTween.To(
                    () => _light.intensity,
                    x => _light.intensity = x,
                    _pulseIntensity,
                    _pulseTime
                ).SetEase(_ease)
            );

            // Return to default
            seq.Append(
                DOTween.To(
                    () => _light.intensity,
                    x => _light.intensity = x,
                    _defaultIntensity,
                    _returnTime
                ).SetEase(_ease)
            );

            _pulseTween = seq;
        }
    }
}