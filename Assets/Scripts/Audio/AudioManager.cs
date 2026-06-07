using UnityEngine;
using DG.Tweening;
using Utilities;

namespace Audio
{
    public class AudioManager : SingletonMonoBehaviourAcrossScenes<AudioManager>
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _themeAudioSource;
        [SerializeField] private AudioSource _sfxAudioSource;

        [Header("Music Clips")]
        [SerializeField] private AudioClip _mainMenuMusic;
        [SerializeField] private AudioClip _tutorialLoop;
        [SerializeField] private AudioClip _gameMusic;
        [SerializeField] private AudioClip _bossIntro;
        [SerializeField] private AudioClip _winMusic;
        [SerializeField] private AudioClip _gameOverMusic;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip _buttonPressed;
        [SerializeField] private AudioClip _smokeTransition;

        [Header("Settings")]
        [SerializeField] private float _fadeDuration = 0.3f;
        [SerializeField] private float _musicVolume = 1f;

        private Sequence _musicSequence;

        protected override void Awake()
        {
            base.Awake();

            // Make sure audio ignores Time.timeScale
            if (_themeAudioSource != null)
                _themeAudioSource.ignoreListenerPause = true;

            if (_sfxAudioSource != null)
                _sfxAudioSource.ignoreListenerPause = true;

            // Ensure global audio is never paused
            AudioListener.pause = false;
        }


        private void Start()
        {
            if (_themeAudioSource == null)
                Debug.LogError("AudioManager: Theme AudioSource is missing!");

            if (_sfxAudioSource == null)
                Debug.LogError("AudioManager: SFX AudioSource is missing!");

            PlayMainMenuInstant();
        }

        private void OnDestroy()
        {
            _musicSequence?.Kill();
        }

        // ---------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------

        private void EnsureMusicSequence()
        {
            if (_musicSequence == null || !_musicSequence.IsActive())
                _musicSequence = DOTween.Sequence().SetUpdate(true);
        }

        // ---------------------------------------------------------
        // Instant Music Switch (no fade)
        // ---------------------------------------------------------

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null) return;

            EnsureMusicSequence();

            _musicSequence.AppendCallback(() =>
            {
                _themeAudioSource.clip = clip;
                _themeAudioSource.volume = _musicVolume;
                _themeAudioSource.Play();
            });
        }

        // ---------------------------------------------------------
        // Fade Music Switch
        // ---------------------------------------------------------

        // Default fade using inspector value
        public void PlayMusicWithFade(AudioClip clip)
            => PlayMusicWithFade(clip, _fadeDuration, 0f);

        // Custom fade + optional delay
        public void PlayMusicWithFade(AudioClip clip, float fadeDuration, float delayBeforeFade = 0f)
        {
            if (clip == null) return;

            EnsureMusicSequence();

            // Fade out current music
            _musicSequence.Append(_themeAudioSource.DOFade(0f, fadeDuration));

            // Optional delay
            if (delayBeforeFade > 0)
                _musicSequence.AppendInterval(delayBeforeFade);

            // Switch clip
            _musicSequence.AppendCallback(() =>
            {
                _themeAudioSource.clip = clip;
                _themeAudioSource.volume = 0f;
                _themeAudioSource.Play();
            });

            // Fade in new music
            _musicSequence.Append(_themeAudioSource.DOFade(_musicVolume, fadeDuration));
        }

        // ---------------------------------------------------------
        // Manual Fade Controls
        // ---------------------------------------------------------

        public void FadeOut(float duration, float delay = 0f)
        {
            EnsureMusicSequence();
            _musicSequence.Append(_themeAudioSource.DOFade(0f, duration).SetDelay(delay));
        }

        public void FadeIn(float duration, float delay = 0f)
        {
            EnsureMusicSequence();

            _musicSequence.AppendCallback(() =>
            {
                if (!_themeAudioSource.isPlaying)
                    _themeAudioSource.Play();
            });

            _musicSequence.Append(_themeAudioSource.DOFade(_musicVolume, duration).SetDelay(delay));
        }

        // ---------------------------------------------------------
        // SFX
        // ---------------------------------------------------------

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;
            _sfxAudioSource.PlayOneShot(clip);
        }

        public void PlayButtonSFX() => PlaySFX(_buttonPressed);
        public void PlaySmokeTransition() => PlaySFX(_smokeTransition);

        // ---------------------------------------------------------
        // Helper / Extension-style Music Methods
        // ---------------------------------------------------------

        public void PlayMainMenuInstant()
            => PlayMusic(_mainMenuMusic);
        public void PlayMainMenuWithFade(float fade, float delay = 0f)
            => PlayMusicWithFade(_mainMenuMusic, fade, delay);
        public void PlayMainMenuWithFade()
            => PlayMusicWithFade(_mainMenuMusic);

        public void PlayTutorialThemeInstant()
            => PlayMusic(_tutorialLoop);
        public void PlayTutorialThemeWithFade(float fade, float delay = 0f)
            => PlayMusicWithFade(_tutorialLoop, fade, delay);
        public void PlayTutorialThemeWithFade()
            => PlayMusicWithFade(_tutorialLoop);

        public void PlayGameThemeInstant()
            => PlayMusic(_gameMusic);
        public void PlayGameThemeWithFade(float fade, float delay = 0f)
            => PlayMusicWithFade(_gameMusic, fade, delay);
        public void PlayGameThemeWithFade()
            => PlayMusicWithFade(_gameMusic);

        public void PlayBossIntroInstant()
            => PlayMusic(_bossIntro);
        public void PlayBossIntroWithFade(float fade, float delay = 0f)
            => PlayMusicWithFade(_bossIntro, fade, delay);
        public void PlayBossIntroWithFade()
            => PlayMusicWithFade(_bossIntro);

        public void PlayWinMusicInstant()
            => PlayMusic(_winMusic);
        public void PlayWinMusicWithFade(float fade, float delay = 0f)
            => PlayMusicWithFade(_winMusic, fade, delay);
        public void PlayWinMusicWithFade()
            => PlayMusicWithFade(_winMusic);

        public void PlayGameOverMusicInstant()
            => PlayMusic(_gameOverMusic);
        public void PlayGameOverMusicWithFade(float fade, float delay = 0f)
            => PlayMusicWithFade(_gameOverMusic, fade, delay);
        public void PlayGameOverMusicWithFade()
            => PlayMusicWithFade(_gameOverMusic);
    }
}
