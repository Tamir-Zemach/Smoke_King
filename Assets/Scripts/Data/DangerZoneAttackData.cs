using System.Collections.Generic;
using Boss.BossAttacks.DangerZoneAttack;
using Enums;
using Structs;
using UnityEngine;
using Utilities;

namespace Data
{
    [CreateAssetMenu]
    public class DangerZoneAttackData : ScriptableObject
    {
        
        
        public float AttackDur = 5;
        public float DelayBetweenZoneChange = 1;
        public float DelayBeforeAttack = 0.5f;

        
        [Header("Visuals")]
        public List<VisualData> Visuals;
        
        private Dictionary<StateType, VisualData> _lookup;
        
        private void OnEnable()
        {
            _lookup = VisualLookupBuilder.Build(Visuals);
        }

        public VisualData GetVisual(StateType type)
        {
            return GenericVisualHelper.Get(_lookup, type);
        }

    }
}