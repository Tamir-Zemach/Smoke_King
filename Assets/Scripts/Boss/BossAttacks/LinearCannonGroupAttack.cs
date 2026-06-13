using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Enums;
using Interfaces;
using UnityEngine;
using Utilities;

namespace Boss.BossAttacks
{
    public class LinearCannonGroupAttack : MonoBehaviour, IBossAttack
    {
        [SerializeField] private int _loopCount = 3;
        
        private readonly List<LinearCannonGroup> _groups = new();
        private Coroutine _routine;

        private void Awake()
        {
            _groups.Clear();
            foreach (Transform child in transform)
            {
                var group = child.GetComponent<LinearCannonGroup>();
                if (group != null)
                    _groups.Add(group);
            }
        }

        public void PreformAttack(Action onAttackFinished)
        {
            if (_routine != null)
                StopCoroutine(_routine);

            _routine = StartCoroutine(AttackRoutine(onAttackFinished));
        }

        private IEnumerator AttackRoutine(Action onAttackFinished)
        {
            // Start with a random state
            StateType currentState = EnumUtility.GetRandomValue<StateType>();

            for (int loop = 0; loop < _loopCount; loop++)
            {
                foreach (var group in _groups)
                {
                    bool finished = false;

                    // Assign the state to this group
                    group.Init(currentState);

                    // Alternate for next group
                    currentState = ToggleState(currentState);

                    group.FireGroup(() => finished = true);

                    yield return new WaitUntil(() => finished);
                }
            }

            onAttackFinished?.Invoke();
            gameObject.SetActive(false);
        }

        private StateType ToggleState(StateType state)
        {
            return state == StateType.State1 ? StateType.State2 : StateType.State1;
        }
    }
}