using System.Collections.Generic;
using Enums;
using Particles;
using UnityEngine;
using Utilities;

namespace ObjectPooling
{
    public class BossParticlePool : SingletonMonoBehaviour<BossParticlePool>
    {
        [System.Serializable]
        public struct ParticleEntry
        {
            public BossParticles Type;
            public BossParticle Prefab;
        }

        public List<ParticleEntry> Particles;

        private Dictionary<BossParticles, ObjectPool<BossParticle>> _pools;

        protected override void Awake()
        {
            base.Awake();
            _pools = new Dictionary<BossParticles, ObjectPool<BossParticle>>();

            foreach (var entry in Particles)
            {
                var pool = new ObjectPool<BossParticle>(
                    entry.Prefab,
                    3,      // initial size
                    4,      // expand size
                    transform
                );

                _pools.Add(entry.Type, pool);
            }
        }

        public void Play(BossParticles type, Vector3 pos)
        {
            var pool = _pools[type];
            var obj = pool.Get();

            obj.PlayAt(pos, p => pool.Return(p));
        }
    }
}