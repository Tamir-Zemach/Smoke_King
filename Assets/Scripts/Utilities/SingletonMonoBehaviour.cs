using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Abstract base class for creating persistent singleton MonoBehaviours.
    /// Ensures only one instance exists and survives scene loads.
    /// </summary>

    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour 
    {
        public static T Instance;

        private void InstantiateOneObject()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
        }

        /// <summary>
        /// Ensures that only one instance of this MonoBehaviour exists across the game lifecycle.
        /// If a duplicate is found, it is destroyed. Otherwise, this instance is preserved between scene loads.
        /// </summary>
        protected virtual void Awake()
        {
            InstantiateOneObject();
        }
    }
}