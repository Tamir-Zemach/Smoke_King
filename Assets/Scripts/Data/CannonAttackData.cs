using System.Collections.Generic;
using Boss.BossAttacks;
using Enums;
using Structs;
using UnityEngine;
using Utilities;

namespace Data
{
    [CreateAssetMenu]
    public class CannonAttackData : ScriptableObject
    {
        [Header("Pattern Settings")] public LinearCannon CannonPrefab;

        public float DelayBetweenCannonSpawns = 1;

        [Header("ParticleHolder Settings")] public float MaxDistance = 20f;

        public float BeamGrowSpeed = 40f;
        public float DelayBeforeFire = 1f;
        public float BeamDuration = 1f;

        [Header("References")] public LayerMask WallLayer;

        [Header("Visuals")] public List<VisualData> Visuals;

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