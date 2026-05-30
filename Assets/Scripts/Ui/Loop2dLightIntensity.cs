using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Ui
{
    public class Loop2dLightIntensity : MonoBehaviour
    {
        [Header("Intensity Settings")]
        [SerializeField] private float _pulseIntensity = 1.3f;
        [SerializeField] private float _pulseTime = 0.15f;
        [SerializeField] private float _returnTime = 0.15f;
        [SerializeField] private Ease _ease = Ease.InOutSine;

        private Light2D _light;
        private Tween _intensityTween;
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
            StartPulse();
        }

        private void OnDisable()
        {
            _intensityTween?.Kill();
        }

        public void StartPulse()
        {
            _intensityTween?.Kill();

            Sequence seq = DOTween.Sequence();

            // Pulse up
            seq.Append(
                DOTween
                    .To(
                        () => _light.intensity,
                        value => _light.intensity = value,
                        _pulseIntensity,
                        _pulseTime
                    )
                    .SetEase(_ease)
            );

            // Return to default
            seq.Append(
                DOTween
                    .To(
                        () => _light.intensity,
                        value => _light.intensity = value,
                        _defaultIntensity,
                        _returnTime
                    )
                    .SetEase(_ease)
            );

            seq.SetLoops(-1, LoopType.Restart);

            _intensityTween = seq;
        }
    }
}