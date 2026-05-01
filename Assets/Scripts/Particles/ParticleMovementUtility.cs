using DG.Tweening;
using UnityEngine;

namespace Particles
{
    public static class ParticleMovementUtility
    {
        public static Tween Shake(
            Transform target,
            float duration,
            float strength,
            int vibrato)
        {
            return target.DOShakePosition(
                duration,
                strength,
                vibrato,
                randomness: 90,
                snapping: false,
                fadeOut: true
            );
        }

        public static Tween Fly(
            Transform target,
            float distance,
            float duration)
        {
            Vector3 startPos = target.position;
            Vector3 targetPos = startPos + target.up * distance;

            return target.DOMove(targetPos, duration)
                .SetEase(Ease.OutQuad);
        }

        public static void ResetPosition(Transform target, Vector3 pos)
        {
            target.position = pos;
        }

        public static Tween MoveInCircle(
            Transform target,
            float radius,
            float speedDegPerSec,
            float duration)
        {
            float angle = 0f;
            Vector3 startLocalPos = target.localPosition;

            return DOTween.To(
                    () => angle,
                    x =>
                    {
                        angle = x;

                        float rad = angle * Mathf.Deg2Rad;
                        Vector3 offset = new Vector3(
                            Mathf.Cos(rad),
                            Mathf.Sin(rad),
                            0f
                        ) * radius;

                        target.localPosition = startLocalPos + offset;
                    },
                    speedDegPerSec * duration,
                    duration
                )
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }

        // ⭐ NEW: Pulse + Fade tween
        public static void PulseAndFade(Transform target, float pulseScale = 1.3f, float pulseTime = 0.15f, float fadeTime = 0.3f)
        {
            // Pulse scale
            target.DOScale(pulseScale, pulseTime)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    // Fade out material if possible
                    var renderer = target.GetComponent<ParticleSystemRenderer>();
                    if (renderer != null)
                    {
                        var mat = renderer.material;
                        if (mat != null && mat.HasProperty("_Color"))
                        {
                            Color c = mat.color;
                            mat.DOColor(new Color(c.r, c.g, c.b, 0f), fadeTime);
                        }
                    }

                    // Optional: shrink back to normal
                    target.DOScale(1f, 0.2f);
                });
        }
        
        public static void KillAllTweens()
        {
            DOTween.KillAll();
        }

        public static void KillTweens(Transform target)
        {
            DOTween.Kill(target);
        }

    }
}
