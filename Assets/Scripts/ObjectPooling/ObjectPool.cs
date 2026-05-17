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

        // Track active objects
        public List<T> ActiveObjects { get; private set; }

        public ObjectPool(T prefab, int initialSize, int maxPoolSize, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
            _maxPoolSize = maxPoolSize;

            _pool = new Queue<T>(initialSize);
            ActiveObjects = new List<T>();

            // Pre‑create objects
            for (var i = 0; i < initialSize; i++)
            {
                var obj = Object.Instantiate(_prefab, _parent);
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        public T Get()
        {
            T obj;

            if (_pool.Count > 0)
            {
                obj = _pool.Dequeue();
            }
            else
            {
                obj = Object.Instantiate(_prefab, _parent);
            }

            obj.gameObject.SetActive(true);
            ActiveObjects.Add(obj);

            return obj;
        }

        public void Return(T obj)
        {
            ActiveObjects.Remove(obj);

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

        // Return all active objects (useful for stopping looping particles)
        public void ReturnAllActive()
        {
            // Copy to avoid modifying while iterating
            var copy = new List<T>(ActiveObjects);

            foreach (var obj in copy)
            {
                Return(obj);
            }
        }
    }
}
