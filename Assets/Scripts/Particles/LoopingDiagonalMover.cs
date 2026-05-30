using DG.Tweening;
using UnityEngine;

namespace Particles
{
    public class LoopingDiagonalMover : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private Transform _toTransform;
        [SerializeField] private float _duration = 0.6f;
        [SerializeField] private float _delay = 0.2f;
        [SerializeField] private Ease _ease = Ease.OutQuad;

        private Sequence _sequence;
        private Vector3 _startPos;


        private void OnEnable()
        {
            _startPos = transform.localPosition;
            StartLoop();
        }

        private void OnDisable()
        {
            _sequence?.Kill();
        }

        public void StartLoop()
        {
            if (_toTransform == null)
            {
                Debug.LogError("LoopingDiagonalMover: Missing transforms!");
                return;
            }

            _sequence?.Kill();

            Vector3 endPos = _toTransform.localPosition;

            // Snap to start
            transform.localPosition = _startPos;

            _sequence = DOTween.Sequence()
                .Append(transform.DOLocalMove(endPos, _duration).SetEase(_ease))
                .AppendCallback(() =>
                {
                    // Teleport instantly back to start
                    transform.localPosition = _startPos;
                })
                .AppendInterval(_delay) // wait before next flight
                .SetLoops(-1, LoopType.Restart); // repeat forever
        }
    }
}