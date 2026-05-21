using System.Collections;
using Boss.BossAttacks;
using Enums;
using Managers.Boss;
using Particles;
using Player;
using Structs;
using Unity.Cinemachine;
using UnityEngine;

namespace Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private PlayerAttackManager _playerAttack;
        [SerializeField] private PlayerMovementManager _playerMovement;
        [SerializeField] private PlayerStateManager _playerState;
        [SerializeField] private BossManager _bossManager;
        [SerializeField] private TutorialUI _ui;
        [SerializeField] private CinemachineCamera _gameplayCam;
        [SerializeField] private CinemachineCamera _playerZoomCamera;

        [Header("State switch smoke")]
        [SerializeField] private GameObject _smokePrefab;
        [SerializeField] private GameObject _smokeSpawnPos;
        [SerializeField] private float _smokeTriggerDistance = 2f;

        private TutorialStep _step = TutorialStep.None;

        private bool _moved;
        private bool _jumped;
        private bool _attacked;
        private bool _upAttacked;
        
        private Transform _playerTransform;

        private void Start()
        {
            if (_playerInput == null)
            {
                _playerInput = FindAnyObjectByType<PlayerInput>();
            }
            _playerInput.OnJump += OnJump;
            _playerInput.OnAttack += OnAttack;
            _playerInput.OnUpAttack += OnUpAttack;

            StartCoroutine(RunTutorial());
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
            BlockAllInput(true);
            _ui.HideAll();

            // -----------------------------
            // PHASE 1A — Move + Jump
            // -----------------------------
            _step = TutorialStep.MoveJumpAndAttacks;
            BlockAllInput(false);
            _playerInput.TutorialBlocker.BlockStateSwitch = true;

            _moved = _jumped = _attacked = _upAttacked = false;

            _ui.ShowMoveJump();

            while (!(_moved && _jumped))
                yield return null;

            _ui.HideAll();
            yield return new WaitForSeconds(0.25f);

            // -----------------------------
            // PHASE 1B — Attack + Up Attack
            // -----------------------------
            _ui.ShowAttack();

            while (!(_attacked && _upAttacked))
                yield return null;

            _ui.HideAll();
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

        private void BlockAllInput(bool block)
        {
            var b = _playerInput.TutorialBlocker;
            b.BlockMovement = block;
            b.BlockJump = block;
            b.BlockAttack = block;
            b.BlockStateSwitch = block;
        }

        private void OnJump()
        {
            if (_step != TutorialStep.MoveJumpAndAttacks) return;
            _jumped = true;
        }

        private void OnAttack()
        {
            if (_step != TutorialStep.MoveJumpAndAttacks) return;
            _attacked = true;
        }

        private void OnUpAttack()
        {
            if (_step != TutorialStep.MoveJumpAndAttacks) return;
            _upAttacked = true;
        }
        
        
        private void Update()
        {
            if (_step == TutorialStep.MoveJumpAndAttacks)
            {
                if (Mathf.Abs(_playerInput.Movement.x) > 0.1f)
                    _moved = true;
            }
        }


        private IEnumerator StateSwitchPhase()
        {
            // 1. Freeze player
            BlockAllInput(true);

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
                var playerPos = _playerMovement.transform.position;

                Vector2 smokePos = mover != null
                    ? (Vector2)mover.transform.position
                    : (Vector2)smoke.transform.position;

                float dist = Vector2.Distance(smokePos, playerPos);

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
                    // FULL FREEZE: stop simulation + stop time-based evolution
                    var main = smokePs.main;
                    main.simulationSpeed = 0f;
                    smokePs.Pause(true);
                }

                var light = cannon.SmokeParticle.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();
                if (light != null)
                    light.enabled = true; // keep visible if you want the smoke to stay lit
            }

            // 5. Slow motion → pause
            yield return StartCoroutine(LerpTimeScale(1f, 0.1f, 0.25f));
            yield return StartCoroutine(LerpTimeScale(0.1f, 0f, 0.15f));

            // -----------------------------
            // 6. CAMERA SWITCH + ZOOM IN
            // -----------------------------
            _playerZoomCamera.Priority = 30;
            yield return new WaitForSecondsRealtime(1f);

            _ui.ShowStateSwitch(true, _playerMovement.transform);

            var blocker = _playerInput.TutorialBlocker;
            blocker.BlockMovement = true;
            blocker.BlockJump = true;
            blocker.BlockAttack = true;
            blocker.BlockStateSwitch = false;

            bool switched = false;
            void OnStateSwitch() => switched = true;

            _playerInput.OnStateSwitch += OnStateSwitch;

            while (!switched)
                yield return null;

            _playerInput.OnStateSwitch -= OnStateSwitch;

            // -----------------------------
            // 7. ZOOM OUT + SWITCH BACK
            // -----------------------------

            _ui.HideAll();
            
            // Restore time scale
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
                main.simulationSpeed = 1f;   // resume time-based evolution
                smokePs.Play(true);          // resume simulation
            }

            // 9. Unblock everything
            BlockAllInput(false);
        }

        private IEnumerator LerpTimeScale(float from, float to, float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float k = t / duration;
                Time.timeScale = Mathf.Lerp(from, to, k);
                yield return null;
            }
            Time.timeScale = to;
        }

        private IEnumerator BossIntroPhase()
        {
            yield return new WaitForSeconds(0.5f);

            _bossManager.SendMessage("PlaySpawnAnim", SendMessageOptions.DontRequireReceiver);

            yield return new WaitForSeconds(1f);
        }

        private StateType GetOppositeState(StateType s)
        {
            return s == StateType.State1 ? StateType.State2 : StateType.State1;
        }
    }
}
