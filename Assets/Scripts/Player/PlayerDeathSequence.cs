using System.Collections;
using System.Collections.Generic;
using Audio;
using Cameras;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Utilities;
using Data;
using Post_Processing;

namespace Player
{
    public class PlayerDeathSequence : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerHealthManager _health;
        [SerializeField] private PlayerMovementManager _movement;
        [SerializeField] private PlayerAnimationManager _animation;
        [SerializeField] private PlayerInput _input;
        [SerializeField] private PlayerStateManager _stateManager;
        [SerializeField] private Rigidbody2D _rb;

        [Header("Visual Root (for shaking)")]
        [SerializeField] private Transform _visual;

        [Header("Death Data")]
        [SerializeField] private PlayerDeathData _deathData;

        [Header("Death FX")]
        [SerializeField] private ParticleSystem _deathParticles;
        [SerializeField] private List<Light2D> _lightsToFadeOut = new List<Light2D>();
        [SerializeField] private List<ParticleSystem> _particlesToStop = new List<ParticleSystem>();
        [SerializeField] private Light2D _endLight;

        private bool _sequenceStarted = false;
        private Tween _shakeTween;

        private void Awake()
        {
            if (_visual == null) Debug.LogWarning("PlayerDeathSequence: Missing Visual Transform");
        }

        private void OnEnable()
        {
            _health.OnDying += StartDeathSequence;
        }

        private void OnDisable()
        {
            _health.OnDying -= StartDeathSequence;
        }

        private void StartDeathSequence()
        {
            if (_sequenceStarted || _health.IsInDeathSequence)
                return;

            _sequenceStarted = true;
            _health.IsInDeathSequence = true;

            StartCoroutine(DeathSequenceRoutine());
        }

        private IEnumerator DeathSequenceRoutine()
        {
            // ---------------------------------------------------------
            // PHASE 1 — INSTANT FREEZE
            // ---------------------------------------------------------
            VignetteFlash.Instance.StartEndlessPulse(Color.red, 0.35f, 1f);

            var blocker = _input.TutorialBlocker;
            blocker.BlockMovement = true;
            blocker.BlockJump = true;
            blocker.BlockAttack = true;
            blocker.BlockStateSwitch = true;

            _animation.FreezeAnimationsImmediate();
            _movement.FreezeFacing();
            _rb.simulated = false;
            _health.IsInvincible = true;

            AudioManager.Instance.PlayGameOverMusicWithFade();

            // ---------------------------------------------------------
            // PHASE 2 — FX (ALL AT ONCE)
            // ---------------------------------------------------------

            if (_deathParticles != null)
            {
                var par = Instantiate(
                    _deathParticles,
                    transform.position + new Vector3(0, _deathData.ParticleYOffset, 0),
                    Quaternion.identity
                );
                par.transform.SetParent(transform);
            }

            CameraShake.Instance.Shake(_deathData.CameraShakeIntensity, _deathData.CameraShakeDuration);

            // 🔥 Start infinite shaking on the visual transform
            _shakeTween = _visual.DOShakePosition(
                duration: 999f,
                strength: new Vector3(_deathData.PlayerShakeIntensity, _deathData.PlayerShakeIntensity, 0f),
                vibrato: 30,
                randomness: 90,
                fadeOut: false
            )
            .SetUpdate(true)
            .SetLoops(-1, LoopType.Restart);

            // Shader desaturation
            Material mat = _stateManager.PlayerMaterial;
            if (mat != null)
            {
                ShaderLerpUtility.LerpFloatUnscaled(mat, "_Color_A_Saturation", 0f, _deathData.DesaturateDuration);
                ShaderLerpUtility.LerpFloatUnscaled(mat, "_Color_B_Saturation", 0f, _deathData.DesaturateDuration);
            }

            yield return new WaitForSecondsRealtime(_deathData.PreLerpDelay);

            // Move player root to center
            transform.DOMove(
                Vector3.zero - new Vector3(0, _deathData.ParticleYOffset, 0),
                _deathData.MoveToCenterDuration
            )
            .SetEase(Ease.InOutQuad)
            .SetUpdate(true);

            float maxPhase2 = Mathf.Max(_deathData.DesaturateDuration, _deathData.MoveToCenterDuration);
            yield return new WaitForSecondsRealtime(maxPhase2);

            // ---------------------------------------------------------
            // PHASE 3 — LIGHT FADE OUT
            // ---------------------------------------------------------

            if (_endLight != null)
                _endLight.enabled = true;

            int lightsDone = 0;

            foreach (var light in _lightsToFadeOut)
            {
                if (light == null)
                {
                    lightsDone++;
                    continue;
                }

                DOTween.To(
                    () => light.intensity,
                    v => light.intensity = v,
                    0f,
                    _deathData.LightFadeDuration
                )
                .SetUpdate(true)
                .OnComplete(() => lightsDone++);
            }

            foreach (var ps in _particlesToStop)
            {
                if (ps != null)
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }

            while (lightsDone < _lightsToFadeOut.Count)
                yield return null;

            // ---------------------------------------------------------
            // PHASE 4 — WAIT UNTIL PLAYER REACHES CENTER
            // ---------------------------------------------------------

            while (Vector3.Distance(transform.position, Vector3.zero - new Vector3(0, _deathData.ParticleYOffset, 0)) > 0.05f)
                yield return null;

            // ---------------------------------------------------------
            // PHASE 5 — DELAY BEFORE END LIGHT COLLAPSE
            // ---------------------------------------------------------

            yield return new WaitForSecondsRealtime(_deathData.EndLightDelay);

            // ---------------------------------------------------------
            // PHASE 6 — CLOSE THE END LIGHT
            // ---------------------------------------------------------

            if (_endLight != null)
            {
                DOTween.To(
                    () => _endLight.pointLightOuterRadius,
                    v => _endLight.pointLightOuterRadius = v,
                    0f,
                    _deathData.EndLightCloseDuration
                )
                .SetEase(Ease.InOutQuad)
                .SetUpdate(true);

                yield return new WaitForSecondsRealtime(_deathData.EndLightCloseDuration);
            }

            // ---------------------------------------------------------
            // PHASE 7 — STOP SHAKE + LOAD SCENE
            // ---------------------------------------------------------

            if (_shakeTween != null)
                _shakeTween.Kill();

            _visual.localPosition = Vector3.zero;

            VignetteFlash.Instance.StopPulse();
            SceneManager.LoadScene(_deathData.GameOverSceneName);
        }
    }
}
