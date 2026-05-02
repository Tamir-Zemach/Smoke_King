using System;
using System.Collections.Generic;
using Data;
using Enums;
using ObjectPooling;
using UnityEngine;
using Utilities;

namespace Boss.BossAttacks
{
    public class LinearCannonGroup : MonoBehaviour
    {
        private readonly List<Transform> _spawnPoints = new();
        private int _activeCannons;
        private Action _onGroupFinished;
        private StateType _groupState;

        private void Awake()
        {
            _spawnPoints.Clear();
            foreach (Transform child in transform)
            {
                _spawnPoints.Add(child);
            }
        }

        public void Init(StateType State)
        {
            _groupState = State;
        }

        public void FireGroup(Action onFinished)
        {
            _onGroupFinished = onFinished;

            _activeCannons = _spawnPoints.Count;

            foreach (var point in _spawnPoints)
            {
                LinearCannon cannon = LinearCannonPool.Instance.Get();

                cannon.Init(
                    point.position,
                    point.rotation,
                    _groupState,
                    () => OnCannonFinished(cannon)
                );
            }
        }

        private void OnCannonFinished(LinearCannon cannon)
        {
            LinearCannonPool.Instance.Return(cannon);

            _activeCannons--;
            if (_activeCannons <= 0)
            {
                _onGroupFinished?.Invoke();
            }
        }
    }
}