using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Ui
{
    public class Light2DFader : MonoBehaviour
    {
        [SerializeField] private Light2D _light;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _delay = 0f;
        [SerializeField] private bool _onstart;

        private Tween _currentTween;

        private void Awake()
        {
            if (_light == null)
                _light = GetComponent<Light2D>();
        }

        private void Start()
        {
            if (_onstart)
            {
                FadeOut();
            }
        }

        /// <summary>
        /// Fades the Light2D intensity to a target value.
        /// </summary>
        public void FadeTo(float targetIntensity)
        {
            if (_light == null)
                return;

            _currentTween?.Kill();

            _currentTween = DOTween.To(
                    () => _light.intensity,
                    x => _light.intensity = x,
                    targetIntensity,
                    _duration
                )
                .SetDelay(_delay)
                .SetEase(Ease.Linear)
                .SetUpdate(true);
        }

        /// <summary>
        /// Convenience: fade the light out (to 0).
        /// </summary>
        public void FadeOut()
        {
            FadeTo(0f);
        }

        /// <summary>
        /// Convenience: fade the light in to a specific intensity.
        /// </summary>
        public void FadeIn(float targetIntensity = 1f)
        {
            FadeTo(targetIntensity);
        }

        public void CancelFade()
        {
            _currentTween?.Kill();
        }
    }
}