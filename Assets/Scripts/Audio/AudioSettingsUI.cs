using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Audio
{
    public class AudioSettingsUI : MonoBehaviour
    {
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;

        
        private void OnEnable()
        {
            StartCoroutine(InitSlidersNextFrame());
        }

        private IEnumerator InitSlidersNextFrame()
        {
            yield return null; // wait 1 frame

            _musicSlider.SetValueWithoutNotify(AudioManager.Instance.MusicVolume);
            _sfxSlider.SetValueWithoutNotify(AudioManager.Instance.SfxVolume);

            _musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
            _sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSfxVolume);
        }




    }

}