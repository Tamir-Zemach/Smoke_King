using System.Collections;
using Audio;
using Boss.BossAttacks;
using Cameras;
using Enums;
using Managers.Boss;
using Particles;
using Player;
using Structs;
using Unity.Cinemachine;
using UnityEngine;
using Ui;
using DG.Tweening;

namespace Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private PlayerAttackManager _playerAttack;
        [SerializeField] private PlayerMovementManager _playerMovement;
        [SerializeField] private PlayerStateManager _playerState;
        [SerializeField] private PlayerAnimationManager _playerAnimation;
        [SerializeField] private BossManager _bossManager;
        [SerializeField] private TutorialUI _ui;
        [SerializeField] private CinemachineCamera _gameplayCam;
        [SerializeField] private CinemachineCamera _playerZoomCamera;
        [SerializeField] private ParticleSystem _bossEntranceParticles;
        [SerializeField] private UiSmokeTransition _uiSmokeTransition;

        [Header("State switch smoke")]
        [SerializeField] private GameObject _smokePrefab;
        [SerializeField] private GameObject _smokeSpawnPos;
        [SerializeField] private float _smokeTriggerDistance = 2f;

        [Header("Debug")]
        [SerializeField] private bool _debugSkipToBossIntro = false;
        private bool _skipRequested = false;


        
        private TutorialStep _step = TutorialStep.None;

        private bool _movedLeft;
        private bool _movedRight;
        private bool _jumped;
        private bool _attacked;
        private bool _upAttacked;
        private bool _stateSwitchPressed;

        private const float UI_HIDE_DELAY = 0.6f;

        private void Start()
        {
            if (_debugSkipToBossIntro)
            {
                StartCoroutine(DebugSkip());
                return;
            }

            if (_playerInput == null)
                _playerInput = FindAnyObjectByType<PlayerInput>();

            _playerInput.OnJump += OnJump;
            _playerInput.OnAttack += OnAttack;
            _playerInput.OnUpAttack += OnUpAttack;
            _playerInput.OnMovePerformed += OnMovePerformed;
            BlockAllInput(true);
            _playerMovement.FreezeFacing();
            _playerAnimation.FreezeAnimationsImmediate();
            StartCoroutine(RunTutorial());
        }
        private IEnumerator AskSkipTutorial()
        {
            bool decided = false;

            Tween t = _ui.ShowSkipTutorial(
                onYes: () =>
                {
                    _skipRequested = true;
                    decided = true;

                    _ui.FadeBackGroundTo(0, 0.3f);
                    CursorController.Instance.HideCursor();

                    
                },
                onNo: () =>
                {
                    _skipRequested = false;
                    decided = true;

                    AudioManager.Instance.FadeOut(0.3f);
                    AudioManager.Instance.PlayTutorialThemeWithFade(0.5f, 0.2f);

                    _ui.FadeBackGroundTo(0.4f, 0.3f);
                    CursorController.Instance.HideCursor();

                }
            );

            if (t != null)
                yield return t.WaitForCompletion();

            while (!decided)
                yield return null;

            _ui.HideSkipTutorial();
        }



        private IEnumerator DebugSkip()
        {
            // Hide UI
            _ui.HideAll();

            // Make sure player is fully unlocked
            BlockAllInput(false);

            // Small delay to let scene initialize
            yield return new WaitForSeconds(0.5f);

            // Run boss intro immediately
            yield return StartCoroutine(BossIntroPhase());

            _step = TutorialStep.Done;
        }



        private void OnDestroy()
        {
            if (_playerInput == null) return;

            _playerInput.OnJump -= OnJump;
            _playerInput.OnAttack -= OnAttack;
            _playerInput.OnUpAttack -= OnUpAttack;
        }

        private IEnumerator RunTutorial()
        {
            _ui.HideAll();
            _uiSmokeTransition.gameObject.SetActive(true);
            _uiSmokeTransition.PlayTransitionToLeft();

            yield return new WaitForSeconds(1.3f);
            
            yield return StartCoroutine(AskSkipTutorial());

            if (_skipRequested)
            {
                // Same behavior as debug skip
                BlockAllInput(false);
                _playerMovement.UnfreezeFacing();
                _playerAnimation.UnfreezeAnimations();
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(BossIntroPhase());
                _step = TutorialStep.Done;
                yield break;
            }
            // -----------------------------
            // PHASE 1A — Move + Jump
            // -----------------------------
            _playerInput.TutorialBlocker.BlockStateSwitch = true;

            _movedLeft = _movedRight = _jumped = _attacked = _upAttacked = false;

            yield return new WaitForSeconds(1f);

            var t1 = _ui.ShowMoveJump();
            if (t1 != null)
                yield return t1.WaitForCompletion();
            _step = TutorialStep.MoveJump;
            BlockAllInput(false);
            _playerMovement.UnfreezeFacing();
            _playerAnimation.UnfreezeAnimations();



            while (!(_movedLeft && _movedRight && _jumped))
                yield return null;


            yield return PulseAndHideUI();
            yield return new WaitForSeconds(0.25f);

            // -----------------------------
            // PHASE 1B — Attack + Up Attack
            // -----------------------------
            var t2 = _ui.ShowAttack();
            if (t2 != null)
                yield return t2.WaitForCompletion();
            
            _step = TutorialStep.Attack;

            while (!(_attacked && _upAttacked))
                yield return null;


            yield return PulseAndHideUI();
            yield return new WaitForSeconds(0.25f);

            // -----------------------------
            // PHASE 2 – State switch
            // -----------------------------
            _step = TutorialStep.StateSwitch;
            yield return StartCoroutine(StateSwitchPhase());

            // -----------------------------
            // PHASE 3 – Boss intro
            // -----------------------------
            _step = TutorialStep.BossIntro;
            yield return StartCoroutine(BossIntroPhase());

            _step = TutorialStep.Done;
        }

        // ---------------------------------------------------------
        // Helper: Pulse current panel before hiding
        // ---------------------------------------------------------
        private IEnumerator PulseAndHideUI()
        {
            Tween t = _ui.PulseCurrentPanel();
            if (t != null)
                yield return t.WaitForCompletion();

            yield return new WaitForSecondsRealtime(UI_HIDE_DELAY);
            _ui.HideAll();
        }

        private void BlockAllInput(bool block)
        {
            var b = _playerInput.TutorialBlocker;
            b.BlockMovement = block;
            b.BlockJump = block;
            b.BlockAttack = block;
            b.BlockStateSwitch = block;
        }

        // -----------------------------
        // INPUT EVENTS WITH UI PULSES
        // -----------------------------
        private void OnJump()
        {
            if (_step != TutorialStep.MoveJump) return;

            _jumped = true;
            _ui.PulseJump();
        }

        private void OnAttack()
        {
            if (_step != TutorialStep.Attack) return;

            _attacked = true;
            _ui.PulseAttack();
        }

        private void OnUpAttack()
        {
            if (_step != TutorialStep.Attack) return;

            _upAttacked = true;
            _ui.PulseUpAttackGroup();
        }

        private void OnMovePerformed(Vector2 v)
        {
            if (_step != TutorialStep.MoveJump) return;

            switch (v.x)
            {
                case > 0.1f:
                    _movedLeft = true;
                    _ui.PulseRight();
                    break;
                case < -0.1f:
                    _movedRight = true;
                    _ui.PulseLeft();
                    break;
            }
        }


        // -----------------------------
        // STATE SWITCH PHASE
        // -----------------------------
        private IEnumerator StateSwitchPhase()
        {
            // 1. Freeze player
            BlockAllInput(true);
            _playerAnimation.FreezeAnimations(0.5f);
            _playerMovement.FreezeFacing();

            // 2. Spawn smoke
            var smoke = Instantiate(_smokePrefab, _smokeSpawnPos.transform.position, Quaternion.identity);

            var cannon = smoke.GetComponent<LinearCannon>();
            if (cannon != null)
            {
                var opposite = GetOppositeState(_playerState.CurrentStateType);
                var visual = cannon.LinearCannonData.GetVisual(opposite);
                cannon.SmokeParticle.Init(visual.Color, visual.Material, visual.Type);
            }

            var mover = smoke.GetComponentInChildren<TutorialSmokeMover>();

            // 3. Wait until smoke is close
            while (smoke != null)
            {
                Vector2 smokePos = mover != null
                    ? (Vector2)mover.transform.position
                    : (Vector2)smoke.transform.position;

                float dist = Vector2.Distance(smokePos, _playerMovement.transform.position);

                if (dist <= _smokeTriggerDistance)
                    break;

                yield return null;
            }

            // 4. Freeze smoke
            ParticleSystem smokePs = null;

            if (mover != null)
                mover.Freeze();

            if (cannon != null)
            {
                ParticleMovementUtility.KillTweens(cannon.SmokeParticle.transform);

                smokePs = cannon.SmokeParticle.GetComponent<ParticleSystem>();
                if (smokePs != null)
                {
                    var main = smokePs.main;
                    main.simulationSpeed = 0f;
                    smokePs.Pause(true);
                }
            }

            // 5. Slow motion → pause
            yield return StartCoroutine(LerpTimeScale(1f, 0.1f, 0.25f));
            yield return StartCoroutine(LerpTimeScale(0.1f, 0f, 0.15f));

            // -----------------------------
            // 6. CAMERA SWITCH + ZOOM IN
            // -----------------------------
            _playerZoomCamera.Priority = 30;
            yield return new WaitForSecondsRealtime(1f);

            var t3 = _ui.ShowStateSwitch(true, _playerMovement.transform);
            if (t3 != null)
                yield return t3.WaitForCompletion();


            var blocker = _playerInput.TutorialBlocker;
            blocker.BlockMovement = true;
            blocker.BlockJump = true;
            blocker.BlockAttack = true;
            blocker.BlockStateSwitch = false;

            _stateSwitchPressed = false;

            void OnStateSwitch()
            {
                _stateSwitchPressed = true;
            }

            _playerInput.OnStateSwitch += OnStateSwitch;

            while (!_stateSwitchPressed)
                yield return null;

            _playerInput.OnStateSwitch -= OnStateSwitch;

            _ui.PulseStateSwitch();

            // -----------------------------
            // 7. ZOOM OUT + SWITCH BACK
            // -----------------------------
            yield return PulseAndHideUI();

            yield return StartCoroutine(LerpTimeScale(0f, 1f, 0.01f));
            yield return new WaitForSecondsRealtime(1f);

            _playerZoomCamera.Priority = 2;
            _gameplayCam.Priority = 30;

            // 8. Let smoke continue
            if (mover != null)
                mover.Unfreeze();

            if (smokePs != null)
            {
                var main = smokePs.main;
                main.simulationSpeed = 1f;
                smokePs.Play(true);
            }

            // 9. Unfreeze player
            BlockAllInput(false);
            _playerAnimation.UnfreezeAnimations();
            _playerMovement.UnfreezeFacing();
        }

        private IEnumerator LerpTimeScale(float from, float to, float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(from, to, t / duration);
                yield return null;
            }
            Time.timeScale = to;
        }

        private IEnumerator BossIntroPhase()
        {
            yield return _ui.FadeBackGroundTo(0f, 0.25f).WaitForCompletion();

            yield return new WaitForSeconds(2f);
            AudioManager.Instance.PlayBossIntroWithFade();
            Instantiate(_bossEntranceParticles, Vector3.zero, Quaternion.Euler(-90, 0, 0));
            CameraShake.Instance.Shake(0.05f, 4);
            yield return new WaitForSeconds(4f);
            _bossManager.gameObject.SetActive(true);

            yield return new WaitForSeconds(2.5f);
            AudioManager.Instance.PlayGameThemeWithFade();
        }

        private StateType GetOppositeState(StateType s)
        {
            return s == StateType.State1 ? StateType.State2 : StateType.State1;
        }
    }
}
