using Data;
using UnityEngine;

namespace Core
{
        public abstract class PlayerHealthBase : HealthBase
        {
            [SerializeField] protected PlayerData PlayerData;

            protected override void Awake()
            {
                base.Awake();

                if (PlayerData == null)
                {
                    Debug.LogWarning("PlayerData is missing on " + gameObject.name);
                }

                // Initialize health from PlayerData
                _maxHealth = PlayerData.MaxHealth;
                _currentHealth = _maxHealth;
            }
        }
}