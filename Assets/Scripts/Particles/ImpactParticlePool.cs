using Enums;
using ObjectPooling;
using UnityEngine;
using Utilities;

namespace Particles
{
    public class ImpactParticlePool : SingletonMonoBehaviour<ImpactParticlePool>
    {
        public ImpactParticle ImpactPrefab;
        public int InitialSize = 10;
        public int MaxSize = 20;

        private ObjectPool<ImpactParticle> _pool;

        protected override void Awake()
        {
            base.Awake();
            if (_pool != null) return;
            _pool = new ObjectPool<ImpactParticle>(
                ImpactPrefab,
                InitialSize,
                MaxSize,
                transform
            );
        }

        public void PlayImpact(Vector3 position, Material material, Color color)
        {
            var obj = _pool.Get();
            obj.gameObject.SetActive(true);
            obj.PlayAt(position, material, color, p => _pool.Return(p));
        }
    }
}