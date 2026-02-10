using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ObjectPooling
{
    public class ObjectPool<T> where T : Component
    {
        private readonly int _maxPoolSize;
        private readonly Transform _parent;
        private readonly Queue<T> _pool;
        private readonly T _prefab;

        public ObjectPool(T prefab, int initialSize, int maxPoolSize, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
            _maxPoolSize = maxPoolSize;
            _pool = new Queue<T>(initialSize);

            for (var i = 0; i < initialSize; i++)
            {
                var obj = Object.Instantiate(_prefab, _parent);
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        public T Get()
        {
            var obj = _pool.Count > 0
                ? _pool.Dequeue()
                : Object.Instantiate(_prefab, _parent);

            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Return(T obj)
        {
            if (_pool.Count < _maxPoolSize)
            {
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
            else
            {
                Object.Destroy(obj.gameObject);
            }
        }
    }
}