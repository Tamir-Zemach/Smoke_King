using System;
using System.Collections;
using Data;
using Enums;
using Interfaces;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;
using Random = UnityEngine.Random;

namespace Boss.BossAttacks.DangerZoneAttack
{
    public class DangerZoneAttack : MonoBehaviour, IBossAttack
    {
        [SerializeField] private DangerZoneAttackData  _data;
        
        [SerializeField] private Transform _zone1SpawnPoint;
        [SerializeField] private Transform _zone2SpawnPoint;
        
        private DangerZone _dangerZone;
        
        private Coroutine _currentCoroutine;
        private StateType _state;


        private void Awake()
        {
            _dangerZone = GetComponentInChildren<DangerZone>();
            Init();
        }
        private void Init()
        {
            _state = EnumUtility.GetRandomValue<StateType>();
            
            _dangerZone.SetState(_state);
            _dangerZone.ActiveZone(false);
        }



        public void PreformAttack(Action onAttackFinished)
        {
            
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }
            
            _currentCoroutine = StartCoroutine(AttackCycle(onAttackFinished));
        }

        private IEnumerator AttackCycle(Action onAttackFinished)
        {
            float timer = 0f;

            // Random starting state already chosen in Init()
            StateType currentState = _state;

            // Start at a random spawn point too
            bool useFirstSpawn = Random.value > 0.5f;
            while (timer < _data.AttackDur)
            {
                // Set state visuals + damage type
                _dangerZone.SetState(currentState);

                // Pick spawn point
                Transform spawn = useFirstSpawn ? _zone1SpawnPoint : _zone2SpawnPoint;
                _dangerZone.transform.position = spawn.position;

                // Show warning
                _dangerZone.ShowIndex(true);
                yield return new WaitForSeconds(_data.DelayBeforeAttack);

                // Activate zone
                _dangerZone.ShowIndex(false);
                _dangerZone.ActiveZone(true);
                yield return new WaitForSeconds(_data.DelayBetweenZoneChange);

                // Deactivate zone
                _dangerZone.ActiveZone(false);

                // Alternate spawn point
                useFirstSpawn = !useFirstSpawn;

                // Alternate state
                currentState = GetNextState(currentState);

                timer += _data.DelayBetweenZoneChange;
            }
            onAttackFinished?.Invoke();
            gameObject.SetActive(false);
        }
        private StateType GetNextState(StateType current)
        {
            return current == StateType.State1 ? StateType.State2 : StateType.State1;
        }
        
    }
}