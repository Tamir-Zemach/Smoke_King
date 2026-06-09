using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Utilities;

namespace Post_Processing
{
    public class VignetteFlash : SingletonMonoBehaviour<VignetteFlash>
    {
        private Volume _volume;
        private Vignette _vignette;

        private Color _defaultColor;
        private float _defaultIntensity;

        protected override void Awake()
        {
            base.Awake();

            _volume = GetComponent<Volume>();
            if (_volume == null)
                return;

            if (!_volume.profile.TryGet(out _vignette))
                return;

            _defaultColor = _vignette.color.value;
            _defaultIntensity = _vignette.intensity.value;
        }

        // ---------------------------------------------------------
        // ONE-SHOT FLASH
        // ---------------------------------------------------------
        public void FlashInColor(Color color, float flashIntensity = 0.55f, float duration = 0.25f)
        {
            if (_vignette == null)
                return;

            DOTween.Kill("VignetteFlashOneShot");

            _vignette.color.Override(color);
            _vignette.intensity.Override(flashIntensity);

            DOTween.To(
                () => _vignette.intensity.value,
                x => _vignette.intensity.Override(x),
                _defaultIntensity,
                duration
            ).SetId("VignetteFlashOneShot");

            DOTween.To(
                () => _vignette.color.value,
                x => _vignette.color.Override(x),
                _defaultColor,
                duration
            ).SetId("VignetteFlashOneShot");
        }

        // ---------------------------------------------------------
        // ENDLESS PULSE
        // ---------------------------------------------------------
        public void StartEndlessPulse(Color color, float flashIntensity = 0.55f, float duration = 0.25f)
        {
            if (_vignette == null)
                return;

            DOTween.Kill("VignettePulse");
            DOTween.Kill("VignetteFlashOneShot");

            _vignette.color.Override(color);

            DOTween.To(
                    () => _vignette.intensity.value,
                    x => _vignette.intensity.Override(x),
                    flashIntensity,
                    duration
                )
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad)
                .SetId("VignettePulse");
        }

        // ---------------------------------------------------------
        // STOP PULSE
        // ---------------------------------------------------------
        public void StopPulse()
        {
            DOTween.Kill("VignettePulse");

            if (_vignette != null)
            {
                _vignette.intensity.Override(_defaultIntensity);
                _vignette.color.Override(_defaultColor);
            }
        }
    }
}
