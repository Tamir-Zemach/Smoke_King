using UnityEngine;
using DG.Tweening;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Music Sources")]
        [SerializeField] private AudioSource _tutorialSource;
        [SerializeField] private AudioSource _gameSource;

        private void Awake()
        {
            _tutorialSource.volume = 0f;
            _gameSource.volume = 0f;
        }

        public void PlayTutorialTheme(float fadeDuration = 1.5f)
        {
            _gameSource.DOFade(0f, fadeDuration)
                .SetUpdate(true); // ignore timescale

            _tutorialSource.time = 0f;
            _tutorialSource.Play();
            _tutorialSource.DOFade(1f, fadeDuration)
                .SetUpdate(true); // ignore timescale
        }

        public void PlayGameTheme(float fadeDuration = 1.5f)
        {
            _tutorialSource.DOFade(0f, fadeDuration)
                .SetUpdate(true); // ignore timescale

            _gameSource.time = 0f;
            _gameSource.Play();
            _gameSource.DOFade(1f, fadeDuration)
                .SetUpdate(true); // ignore timescale
        }
    }
}