using UnityEngine;
using Enums;
using ObjectPooling;
using System.Collections.Generic;

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

        [SerializeField] private List<BossParticleSpawnPoint> _spawnPoints;

        private Dictionary<BossParticles, BossParticleSpawnPoint> _lookup;

        private void Awake()
        {
            _lookup = new Dictionary<BossParticles, BossParticleSpawnPoint>();

            foreach (var entry in _spawnPoints)
                _lookup[entry.Type] = entry;
        }

        public void PlayParticles(BossParticles type)
        {
            var data = _lookup[type];

            Vector3 pos = data.SpawnPoint.position;

            if (_movementManager.IsCurrentSpotLeft)
                pos += data.Offset;

            BossParticlePool.Instance.Play(type, pos);
        }


    }
}