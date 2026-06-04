using DG.Tweening;
using UnityEngine;

namespace Audio
{
    public class AudioFadeController : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _targetVolume = 1f;
        [SerializeField] private float _delay = 0f;

        private Tween _currentTween;

        private void Awake()
        {
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();
        }

        public void FadeOut()
        {
            if (_audioSource == null)
                return;

            _currentTween?.Kill();

            _currentTween = _audioSource
                .DOFade(0f, _duration)
                .SetDelay(_delay)
                .SetEase(Ease.Linear)
                .OnComplete(() => _audioSource.Stop());
        }

        public void FadeIn()
        {
            if (_audioSource == null)
                return;

            _currentTween?.Kill();

            if (!_audioSource.isPlaying)
            {
                _audioSource.volume = 0f;
                _audioSource.Play();
            }

            _currentTween = _audioSource
                .DOFade(_targetVolume, _duration)
                .SetDelay(_delay)
                .SetEase(Ease.Linear);
        }

        public void CancelFade()
        {
            _currentTween?.Kill();
        }
    }
}