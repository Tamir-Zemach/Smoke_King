using UnityEngine;
using Enums;
using ObjectPooling;
using System.Collections.Generic;
using Particles;

namespace Managers.Boss
{
    public class BossParticlesManager : MonoBehaviour
    {
        [SerializeField] private BossMovementManager _movementManager;

        [System.Serializable]
        public struct BossParticleSpawnPoint
        {
            public BossParticles Type;
            public Transform SpawnPoint;
            public Vector3 Offset;
        }

        [System.Serializable]
        public struct StateHitColor
        {
            public StateType State;
            public Color Color;
        }

        [SerializeField] private List<StateHitColor> _stateColors;
        private Dictionary<StateType, Color> _colorLookup;

        [SerializeField] private List<BossParticleSpawnPoint> _spawnPoints;
        private Dictionary<BossParticles, BossParticleSpawnPoint> _lookup;

        private void Awake()
        {
            _lookup = new Dictionary<BossParticles, BossParticleSpawnPoint>();
            _colorLookup = new Dictionary<StateType, Color>();

            foreach (var entry in _spawnPoints)
                _lookup[entry.Type] = entry;

            foreach (var entry in _stateColors)
                _colorLookup[entry.State] = entry.Color;
        }

        public void PlayParticles(BossParticles type, Vector3? customPos = null, StateType? state = null)
        {
            Vector3 pos;

            if (customPos.HasValue)
                pos = customPos.Value;
            else
            {
                var data = _lookup[type];
                pos = data.SpawnPoint.position;

                if (_movementManager.IsCurrentSpotLeft)
                    pos += data.Offset;
            }

            var pool = BossParticlePool.Instance;

            // HIT particles get recolored
            if (type == BossParticles.Hit && state.HasValue)
            {
                if (_colorLookup.TryGetValue(state.Value, out var color))
                {
                    var objectPool = pool.GetPool(type);

                    var particle = objectPool.Get();

                    particle.SetStartColor(color);
                    particle.PlayAt(pos, p => objectPool.Return(p));

                    return;
                }

            }


            // Normal particles
            pool.Play(type, pos);
        }
    }
}
