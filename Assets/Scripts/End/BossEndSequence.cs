using System.Collections;
using Audio;
using Cameras;
using Managers;
using Managers.Boss;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Ui;
using UnityEngine.Rendering.Universal;

namespace End
{
    public class BossEndSequence : MonoBehaviour
    {
        [SerializeField] private BossAnimManager _anim;
        [SerializeField] private BossManager _bossManager;
        [SerializeField] private GameObject _bossRoot;
        [SerializeField] private GameObject _endPngPrefab;
        [SerializeField] private GameObject _endPngPos;
        [SerializeField] private Light2D _flashLight;
        [SerializeField] private ParticleSystem _spawnParticles;
        [SerializeField] private ParticleSystem _backgroundParticles;

        [SerializeField] private float _revealDuration = 6f;
        [SerializeField] private string _finalSceneName = "GameWin";

        private bool _endSequenceStarted = false;

        private void Start()
        {
            GameManager.Instance.OnWinGame += StartEndSequence;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnWinGame -= StartEndSequence;

            _anim.OnTeleportAnimEnds -= OnTeleportFinished;
        }

        private void StartEndSequence()
        {
            if (_endSequenceStarted)
                return;

            _endSequenceStarted = true;

            _bossManager.StartEndSequence();
            _anim.TriggerTeleportAnimation();
            _anim.OnTeleportAnimEnds += OnTeleportFinished;
        }

        private void OnTeleportFinished()
        {
            _anim.OnTeleportAnimEnds -= OnTeleportFinished;

            _bossRoot.SetActive(false);
            Instantiate(_spawnParticles, Vector3.zero, Quaternion.Euler(-90, 0, 0));

            StartCoroutine(SpawnAndReveal());
        }

        private IEnumerator SpawnAndReveal()
        {
            yield return new WaitForSeconds(4f);
            Instantiate(_backgroundParticles, new Vector3(0, 0.75f, 0), Quaternion.Euler(-90, 0, 0));
            
            GameObject png = Instantiate(
                _endPngPrefab,
                _endPngPos.transform.position,
                Quaternion.identity
            );

            var renderer = png.GetComponent<SpriteRenderer>();
            var mat = renderer.material;

            var pipe = png.GetComponentInChildren<ShakeFlyAndTint>();
            if (pipe != null)
                pipe.PlayEffect();

            CameraShake.Instance.Shake(0.05f, _revealDuration + 1);

            // Start values
            //mat.SetFloat("_Reveal", 0f);
            _flashLight.intensity = 0f;

            // DOTween sequence
            Sequence seq = DOTween.Sequence();

// Reveal PNG immediately
            seq.Join(
                DOTween.To(
                        () => mat.GetFloat("_Reveal"),
                        value => mat.SetFloat("_Reveal", value),
                        1f,
                        _revealDuration
                    )
                    .SetEase(Ease.Linear)
            );
            AudioManager.Instance.FadeOut(_revealDuration);
            
// Flash the 2D light with a delay
            seq.Join(
                DOTween.To(
                        () => _flashLight.intensity,
                        value => _flashLight.intensity = value,
                        40f,
                        _revealDuration * 0.8f
                    )
                    .SetEase(Ease.InOutQuad)
                    .SetDelay(4f) // ⭐ light starts 1 second after reveal begins
            );

// Load scene when done
            seq.OnComplete(() =>
            {
                CursorController.Instance.ShowCursor();
                AudioManager.Instance.PlayWinMusicWithFade();

                SceneManager.LoadScene(_finalSceneName);
            });

        }
    }
}
