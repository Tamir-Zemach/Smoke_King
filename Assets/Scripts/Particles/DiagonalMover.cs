using DG.Tweening;
using UnityEngine;

namespace Particles
{
    public class DiagonalMover : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private Transform _fromTransform;
        [SerializeField] private Transform _toTransform;
        [SerializeField] private float _duration = 0.6f;
        [SerializeField] private Ease _ease = Ease.OutQuad;

        private Tween _currentTween;

        public void Move()
        {
            if (_fromTransform == null || _toTransform == null)
            {
                Debug.LogError("DiagonalMover: Missing transforms!");
                return;
            }

            _currentTween?.Kill();

            // LOCAL positions (important!)
            Vector3 startLocalPos = _fromTransform.localPosition;
            Vector3 endLocalPos = _toTransform.localPosition;

            // Snap to start
            transform.localPosition = startLocalPos;

            // Tween in LOCAL space
            _currentTween = transform.DOLocalMove(endLocalPos, _duration)
                .SetEase(_ease)
                .OnComplete(() =>
                {
                    // Snap back instantly
                    transform.localPosition = startLocalPos;
                });
        }
    }
}