using System.Collections.Generic;
using Enums;
using Structs;
using UnityEngine;
using Utilities;

namespace Data
{
    [CreateAssetMenu]
    public class PlayerData : ScriptableObject
    {
        [Header("Movement Parameters")] public float Speed = 5;
        public float JumpForce = 200;
        public float JumpForceFromWallY = 200;
        public float JumpForceFromWallX = 200;
        public LayerMask GroundMask;
        public LayerMask WallMask;

        [Header("Attack Parameters")] 
        public float AttackDuration = 0.2f;

        [Header("Health Parameters")] public int MaxHealth = 3;

        [Header("Getting Hit Parameters")] 
        public float KnockBackForce = 15;
        public float EaseControlBackDur = 0.3f;

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