using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class HeartUI : MonoBehaviour
    {
        [SerializeField] private Image _fill;

        private Tween _currentTween;

        public void SetFull()
        {
            _currentTween?.Kill();
            _fill.transform.localScale = Vector3.one;
            _fill.color = new Color(_fill.color.r, _fill.color.g, _fill.color.b, 1f);
            _fill.enabled = true;
        }

        public void SetEmptyInstant()
        {
            _currentTween?.Kill();
            _fill.enabled = false;
        }

        public Tween AnimateLoseHeart()
        {
            _currentTween?.Kill();
            _fill.enabled = true;

            // Step 1: Pulse
            Tween pulse = UiMovementUtility.Pulse(
                _fill.transform,
                pulseScale: 1.25f,
                pulseTime: 0.15f
            );

            // Step 2: Fade + shrink AFTER pulse completes
            pulse.OnComplete(() =>
            {
                UiMovementUtility.ShrinkTo(
                    _fill.transform,
                    shrinkTime: 0.2f,
                    shrinkScale: 0f
                );
            });

            return pulse;
        }

    }
}