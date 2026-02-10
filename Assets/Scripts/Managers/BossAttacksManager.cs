using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Interfaces;
using Structs;
using UnityEngine;

namespace Managers
{
    public class BossAttacksManager : MonoBehaviour
    {
        public Action OnAttackFinished; 
        public List<BossAttack> BossAttacks; 

        private Dictionary<BossAttacksTypes, GameObject> _attackInstances;
        
        public void Init()
        {
            _attackInstances = new Dictionary<BossAttacksTypes, GameObject>();

            foreach (var attack in BossAttacks)
            {
                var instance = Instantiate(attack.Prefab, transform.position, transform.rotation);
                instance.SetActive(false);
                instance.transform.SetParent(transform);

                _attackInstances.Add(attack.AttacksType, instance);
                
            }
        }
        
        public void TriggerAttack(BossAttacksTypes attacksType)
        {
            var attackInstance = _attackInstances[attacksType];

            attackInstance.SetActive(true);

            if (attackInstance.TryGetComponent(out IBossAttack attack))
            {
                attack.PreformAttack(OnAttackFinished);
            }
        }
        
        
    }
}