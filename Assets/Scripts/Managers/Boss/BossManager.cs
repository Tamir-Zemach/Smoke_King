using System.Collections;
using Boss;
using Enums;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Managers.Boss
{
    public class BossManager : MonoBehaviour
    {
        [SerializeField] private BossAttacksManager _attacksManager;
        [SerializeField] private BossMovementManager _movementManager;
        [SerializeField] private BossAnimManager _animManager;
        [SerializeField] private BossParticlesManager _particlesManager;
        [SerializeField] private BossHealthManager _healthManager;


        private BossAttacksTypes _currentBossAttack;


        private void Awake()
        {
            _attacksManager.Init();
            SubscribeToEvents();
        }

        private void Start()
        {
            _animManager.TriggerTeleportAnimation();
            StartCoroutine(InitialSpawn());
        }

        private void OnDestroy()
        {
            SubscribeToEvents(true);
        }

        private void SubscribeToEvents(bool unsubscribe = false)
        {
            if (!_animManager || !_attacksManager) return;
            if (unsubscribe)
            {
                _animManager.OnSpawnAnimEnds -= NextAttack;
                _animManager.OnStartAttack -= StartTimerAndTriggerAttack;
                _animManager.OnTeleportTimerEnd -= StartTeleportAnim;

                _animManager.OnTeleportAnimEnds -= SpawnAtRandomSpot;
                _attacksManager.OnAttackFinished -= PlaySpawnAnim;

                _animManager.OnParticlesPlay -= PlayParticles;
                _healthManager.OnBossHit -= PlayHitParticles;

                return;
            }

            _animManager.OnSpawnAnimEnds += NextAttack;
            _animManager.OnStartAttack += StartTimerAndTriggerAttack;
            _animManager.OnTeleportTimerEnd += StartTeleportAnim;

            _animManager.OnTeleportAnimEnds += SpawnAtRandomSpot;
            _attacksManager.OnAttackFinished += PlaySpawnAnim;

            _animManager.OnParticlesPlay += PlayParticles;
            _healthManager.OnBossHit += PlayHitParticles;
        }


        private IEnumerator InitialSpawn()
        {
            yield return new WaitForSeconds(3);
            PlaySpawnAnim();
        }

        private void NextAttack()
        {
            _currentBossAttack = EnumUtility.GetNextValueInEnum(_currentBossAttack);
            _animManager.TriggerAttackAnimation(_currentBossAttack);
        }


        private void StartTimerAndTriggerAttack(BossAttacksTypes attack)
        {
            _animManager.StartTimerToTeleport(2f);
            _attacksManager.TriggerAttack(attack);
        }

        private void StartTeleportAnim()
        {
            _animManager.TriggerTeleportAnimation();
        }


        private void SpawnAtRandomSpot()
        {
            var randomIndex = Random.Range(0, _movementManager.TeleportSpots.Count);
            _movementManager.TeleportToSpot(randomIndex);
        }

        private void PlaySpawnAnim()
        {
            _animManager.TriggerSpawnAnimation();
        }

        private void PlayHitParticles()
        {
            _particlesManager.PlayParticles(BossParticles.Hit);
        }

        private void PlayParticles(BossParticles bossParticles)
        {
            _particlesManager.PlayParticles(bossParticles);
        }
    }
}