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
        [Header("Movement Parameters")] public float Speed = 6;
        public float JumpForce = 200;
        public float JumpForceFromWallY = 160;
        public float JumpForceFromWallX = 130;
        public LayerMask GroundMask;
        public LayerMask WallMask;

        [Header("Attack Parameters")] 
        public float AttackDuration = 0.5f;
        public float DelayBeforeHitBox = 0.1667f;

        [Header("Health Parameters")] public int MaxHealth = 5;

        [Header("Getting Hit Parameters")] 
        public float KnockBackForce = 100;
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