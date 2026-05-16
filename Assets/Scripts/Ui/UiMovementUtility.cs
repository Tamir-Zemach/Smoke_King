using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public static class UiMovementUtility
    {
        /// <summary>
        /// Only pulse (scale up relative to current scale)
        /// </summary>
        public static Tween Pulse(
            Transform target,
            float pulseScale = 1.3f,
            float pulseTime = 0.15f)
        {
            float startScale = target.localScale.x;

            if (startScale <= 0f)
                startScale = 0.001f;

            float endScale = startScale * pulseScale;

            return target
                .DOScale(endScale, pulseTime)
                .SetEase(Ease.OutQuad);
        }

        /// <summary>
        /// Only fade the Image
        /// </summary>
        public static Tween Fade(
            Image target,
            float fadeTime = 0.3f)
        {
            return target.DOFade(0f, fadeTime);
        }

        /// <summary>
        /// Only shrink back to normal scale (1)
        /// </summary>
        public static Tween ShrinkTo(
            Transform target,
            float shrinkTime = 0.2f,
            float shrinkScale = 0.3f)
        {
            return target
                .DOScale(shrinkScale, shrinkTime)
                .SetEase(Ease.OutQuad);
        }
    }
}