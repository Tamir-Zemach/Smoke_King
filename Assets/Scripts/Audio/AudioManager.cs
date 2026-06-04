using UnityEngine;
using DG.Tweening;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Source")]
        [SerializeField] private AudioSource _musicSource;

        [Header("Music Clips")]
        [SerializeField] private AudioClip _tutorialClip;
        [SerializeField] private AudioClip _gameClip;
        [SerializeField] private AudioClip _bossClip;

        [Header("Settings")]
        [SerializeField] private float _fadeDuration = 0.3f;
        [SerializeField] private float _delay = 0f;

        private Tween _currentTween;

        private void Awake()
        {
            if (_musicSource == null)
                _musicSource = GetComponent<AudioSource>();

            _musicSource.volume = 0f;
        }

        // -------------------------
        // Fade In / Fade Out
        // -------------------------

        public void FadeIn(float fadeDuration)
        {
            _currentTween?.Kill();

            if (!_musicSource.isPlaying)
            {
                _musicSource.volume = 0f;
                _musicSource.Play();
            }

            _currentTween = _musicSource
                .DOFade(1f, fadeDuration)
                .SetDelay(_delay)
                .SetEase(Ease.Linear)
                .SetUpdate(true);
        }

        public void FadeOut(float fadeDuration)
        {
            _currentTween?.Kill();

            _currentTween = _musicSource
                .DOFade(0f, fadeDuration)
                .SetDelay(_delay)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .OnComplete(() => _musicSource.Stop());
        }

        // -------------------------
        // Play Music (Crossfade)
        // -------------------------

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null)
                return;

            _currentTween?.Kill();

            // If nothing is playing, fade in
            if (!_musicSource.isPlaying)
            {
                _musicSource.clip = clip;
                _musicSource.volume = 0f;
                _musicSource.Play();

                _currentTween = _musicSource
                    .DOFade(1f, _fadeDuration)
                    .SetDelay(_delay)
                    .SetUpdate(true);

                return;
            }

            // Crossfade
            Sequence seq = DOTween.Sequence().SetUpdate(true);

            seq.Append(_musicSource.DOFade(0f, _fadeDuration))
               .AppendCallback(() =>
               {
                   _musicSource.clip = clip;
                   _musicSource.Play();
               })
               .Append(_musicSource.DOFade(1f, _fadeDuration));

            _currentTween = seq;
        }

        // Convenience methods
        public void PlayTutorialTheme() => PlayMusic(_tutorialClip);
        public void PlayGameTheme() => PlayMusic(_gameClip);
        public void PlayBossIntro() => PlayMusic(_bossClip);
    }
}
