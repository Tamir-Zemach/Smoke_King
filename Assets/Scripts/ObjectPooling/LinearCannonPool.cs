using Boss.BossAttacks;
using Utilities;

namespace ObjectPooling
{
    public class LinearCannonPool : SingletonMonoBehaviour<LinearCannonPool>
    {
        public LinearCannon CannonPrefab;
        public int InitialSize = 10;
        public int MaxSize = 30;

        private ObjectPool<LinearCannon> _pool;

        protected override void Awake()
        {
            base.Awake();
            if (_pool != null) return;

            _pool = new ObjectPool<LinearCannon>(
                CannonPrefab,
                InitialSize,
                MaxSize,
                transform
            );
        }

        public LinearCannon Get()
        {
            return _pool.Get();
        }

        public void Return(LinearCannon cannon)
        {
            _pool.Return(cannon);
        }
    }
}