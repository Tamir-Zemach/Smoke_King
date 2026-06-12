using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Enums;
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
        [SerializeField] private AudioClip _playerDeathSequence;

        [Header("SFX Clips")]
        [SerializeField] private List<SfxEntry> _sfxEntries;

        [Header("Settings")]
        [SerializeField] private float _fadeDuration = 0.3f;
        public float MusicVolume { get; private set; } = 1f;
        public float SfxVolume { get; private set; } = 0.7f;


        private Dictionary<SfxType, SfxEntry> _sfxMap;
        private Dictionary<SfxType, float> _lastSfxTime = new();
        private Sequence _musicSequence;

        protected override void Awake()
        {
            base.Awake();
            BuildSfxDictionary();
        }

        private void Start()
        {
            if (_themeAudioSource == null)
            {
                Debug.LogError("AudioManager: Theme AudioSource is missing!");
            }

            if (_sfxAudioSource == null)
            {
                Debug.LogError("AudioManager: SFX AudioSource is missing!");
            }
            
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.7f);
            _themeAudioSource.volume = MusicVolume;
            _sfxAudioSource.volume = SfxVolume;

            _themeAudioSource.volume = MusicVolume;
            PlayMainMenuInstant();
        }

        private void OnDestroy()
        {
            _musicSequence?.Kill();
        }

        // ---------------------------------------------------------
        // Build SFX dictionary
        // ---------------------------------------------------------
        private void BuildSfxDictionary()
        {
            _sfxMap = new Dictionary<SfxType, SfxEntry>();

            foreach (var entry in _sfxEntries)
            {
                if (!_sfxMap.ContainsKey(entry.Type))
                    _sfxMap.Add(entry.Type, entry);
            }
        }
        
        public void SetMusicVolume(float value)
        {
            MusicVolume = value;
            _themeAudioSource.volume = value;
            PlayerPrefs.SetFloat("MusicVolume", value);
            PlayerPrefs.Save(); // <-- important
        }

        public void SetSfxVolume(float value)
        {
            SfxVolume = value;
            PlayerPrefs.SetFloat("SfxVolume", value);
            PlayerPrefs.Save(); // <-- important
        }


        
        
        
        
        // ---------------------------------------------------------
        // Music Helpers
        // ---------------------------------------------------------
        private void EnsureMusicSequence()
        {
            if (_musicSequence == null || !_musicSequence.IsActive())
                _musicSequence = DOTween.Sequence().SetUpdate(true);
        }

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null) return;

            EnsureMusicSequence();

            _musicSequence.AppendCallback(() =>
            {
                _themeAudioSource.clip = clip;
                _themeAudioSource.volume = MusicVolume;
                _themeAudioSource.Play();
            });
        }

        public void PlayMusicWithFade(AudioClip clip)
            => PlayMusicWithFade(clip, _fadeDuration, 0f);

        public void PlayMusicWithFade(AudioClip clip, float fadeDuration, float delayBeforeFade = 0f)
        {
            if (clip == null) return;

            EnsureMusicSequence();

            _musicSequence.Append(_themeAudioSource.DOFade(0f, fadeDuration));

            if (delayBeforeFade > 0)
                _musicSequence.AppendInterval(delayBeforeFade);

            _musicSequence.AppendCallback(() =>
            {
                _themeAudioSource.clip = clip;
                _themeAudioSource.volume = 0f;
                _themeAudioSource.Play();
            });

            _musicSequence.Append(_themeAudioSource.DOFade(MusicVolume, fadeDuration));
        }

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

            _musicSequence.Append(_themeAudioSource.DOFade(MusicVolume, duration).SetDelay(delay));
        }

        // ---------------------------------------------------------
        // SFX
        // ---------------------------------------------------------
        public void PlaySfx(SfxType type, bool canOverlap = true)
        {
            if (!_sfxMap.TryGetValue(type, out var entry) || entry.Clip == null)
                return;

            float now = Time.time;

            // Only apply cooldown if overlapping is NOT allowed
            if (!canOverlap)
            {
                if (_lastSfxTime.TryGetValue(type, out float lastTime))
                {
                    if (now - lastTime < entry.Cooldown)
                        return; // too soon → skip
                }

                _lastSfxTime[type] = now;
            }

            float finalVolume = SfxVolume * entry.Volume;
            _sfxAudioSource.PlayOneShot(entry.Clip, finalVolume);
        }






        // ---------------------------------------------------------
        // Music Shortcuts
        // ---------------------------------------------------------
        public void PlayMainMenuInstant() => PlayMusic(_mainMenuMusic);
        public void PlayMainMenuWithFade(float fade, float delay = 0f) => PlayMusicWithFade(_mainMenuMusic, fade, delay);
        public void PlayMainMenuWithFade() => PlayMusicWithFade(_mainMenuMusic);

        public void PlayTutorialThemeInstant() => PlayMusic(_tutorialLoop);
        public void PlayTutorialThemeWithFade(float fade, float delay = 0f) => PlayMusicWithFade(_tutorialLoop, fade, delay);
        public void PlayTutorialThemeWithFade() => PlayMusicWithFade(_tutorialLoop);

        public void PlayGameThemeInstant() => PlayMusic(_gameMusic);
        public void PlayGameThemeWithFade(float fade, float delay = 0f) => PlayMusicWithFade(_gameMusic, fade, delay);
        public void PlayGameThemeWithFade() => PlayMusicWithFade(_gameMusic);

        public void PlayBossIntroInstant() => PlayMusic(_bossIntro);
        public void PlayBossIntroWithFade(float fade, float delay = 0f) => PlayMusicWithFade(_bossIntro, fade, delay);
        public void PlayBossIntroWithFade() => PlayMusicWithFade(_bossIntro);

        public void PlayWinMusicInstant() => PlayMusic(_winMusic);
        public void PlayWinMusicWithFade(float fade, float delay = 0f) => PlayMusicWithFade(_winMusic, fade, delay);
        public void PlayWinMusicWithFade() => PlayMusicWithFade(_winMusic);

        public void PlayGameOverMusicInstant() => PlayMusic(_gameOverMusic);
        public void PlayGameOverMusicWithFade(float fade, float delay = 0f) => PlayMusicWithFade(_gameOverMusic, fade, delay);
        public void PlayGameOverMusicWithFade() => PlayMusicWithFade(_gameOverMusic);

        public void PlayPlayerDeathSequenceWithFade() => PlayMusicWithFade(_playerDeathSequence);
    }
}
