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
        public static Tween FlyTo(
            Transform target,
            Vector2 dir,
            float distance,
            float duration)
        {
            Vector3 startPos = target.position;

            // Normalize direction to avoid speed differences
            Vector3 direction = new Vector3(dir.x, dir.y, 0f).normalized;

            Vector3 targetPos = startPos + direction * distance;

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
                .SetEase(Ease.Linear); // ← NO LOOPS
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
