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
            {
                Debug.LogError("VignetteFlash requires a Volume component.");
                return;
            }

            if (!_volume.profile.TryGet(out _vignette))
            {
                Debug.LogError("VignetteFlash: No Vignette override found in Volume.");
                return;
            }

            _defaultColor = _vignette.color.value;
            _defaultIntensity = _vignette.intensity.value;
        }

        public void FlashRed(float flashIntensity = 0.55f, float duration = 0.25f)
        {
            if (_vignette == null)
                return;

            // Flash to red
            _vignette.color.value = Color.red;
            _vignette.intensity.value = flashIntensity;

            // Tween back to default
            DOTween.To(
                () => _vignette.intensity.value,
                x => _vignette.intensity.value = x,
                _defaultIntensity,
                duration
            );

            DOTween.To(
                () => _vignette.color.value,
                x => _vignette.color.value = x,
                _defaultColor,
                duration
            );
        }
    }
}