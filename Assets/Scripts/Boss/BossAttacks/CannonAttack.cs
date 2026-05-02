using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Enums;
using Interfaces;
using ObjectPooling;
using UnityEngine;
using Utilities;

namespace Boss.BossAttacks
{
    public class CannonAttack : MonoBehaviour, IBossAttack
    {
        public bool Tracking;
        [SerializeField] private CannonAttackData _attackData;
        [SerializeField] private List<Transform> _cannonSpawnPoints;

        private Coroutine _currentCoroutine;

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
            foreach (var point in _cannonSpawnPoints)
            {
                var randomState = EnumUtility.GetRandomValue<StateType>();

                LinearCannon cannon;
                if (Tracking)
                {
                    cannon = TrackingCannonPool.Instance.Get();
                    cannon.Init(
                        point.position,
                        point.rotation,
                        randomState,
                        () => TrackingCannonPool.Instance.Return(cannon)
                    );

                }
                else
                {
                    cannon = LinearCannonPool.Instance.Get();
                    cannon.Init(
                        point.position,
                        point.rotation,
                        randomState,
                        () => LinearCannonPool.Instance.Return(cannon)
                    );
                }
                


                yield return new WaitForSeconds(_attackData.DelayBetweenCannonSpawns);
            }

            onAttackFinished?.Invoke();
            gameObject.SetActive(false);
        }
    }
}