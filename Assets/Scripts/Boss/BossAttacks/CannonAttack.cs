using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Interfaces;
using ObjectPooling;
using UnityEngine;

namespace Boss.BossAttacks
{
    //uniTask, addressables 

    public class CannonAttack : MonoBehaviour, IBossAttack
    {
        [SerializeField] private CannonAttackData _attackData;
        [SerializeField] private List<Transform> _cannonSpawnPoints;

        private Coroutine _currentCoroutine;

        private ObjectPool<LinearCannon> _linearCannonPool;
        private void OnEnable()
        {
            Init();
        }

        public void PreformAttack(Action onAttackFinished)
        {
            Init(); // guard in case OnEnable hasn’t run yet

            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }

            _currentCoroutine = StartCoroutine(AttackCycle(onAttackFinished));
        }

        private void Init()
        {
            if (_linearCannonPool != null) return;
            var parent = new GameObject("LinearCannon pool");
            _linearCannonPool = new ObjectPool<LinearCannon>(_attackData.CannonPrefab, _cannonSpawnPoints.Count,
                _cannonSpawnPoints.Count * 2, parent.transform);
        }

        private IEnumerator AttackCycle(Action onAttackFinished)
        {
            foreach (var point in _cannonSpawnPoints)
            {
                var nextLaser = _linearCannonPool.Get();
                nextLaser.Init(point.position, point.rotation, () => _linearCannonPool.Return(nextLaser));

                yield return new WaitForSeconds(_attackData.DelayBetweenCannonSpawns);
            }

            onAttackFinished?.Invoke();
            gameObject.SetActive(false);
        }
    }
}