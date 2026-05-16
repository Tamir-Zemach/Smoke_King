using System.Collections.Generic;
using Cameras;
using DG.Tweening;
using Player;
using Post_Processing;
using UnityEngine;

namespace Ui
{
    public class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField] private PlayerHealthManager _playerHealthManager;
        [SerializeField] private HeartUI _heartPrefab;
        [SerializeField] private Transform _heartsParent;

        private List<HeartUI> _hearts = new List<HeartUI>();
        private int _lastHealth;

        private void Awake()
        {
            _playerHealthManager.OnHealthChanged += UpdateHearts;
        }

        private void Start()
        {
            CreateHearts();
            _lastHealth = _playerHealthManager.CurrentHealth;
            UpdateHearts();
        }

        private void OnDestroy()
        {
            _playerHealthManager.OnHealthChanged -= UpdateHearts;
        }

        private void CreateHearts()
        {
            for (int i = 0; i < _playerHealthManager.MaxHealth; i++)
            {
                HeartUI heart = Instantiate(_heartPrefab, _heartsParent);
                _hearts.Add(heart);
            }
        }

        private void UpdateHearts()
        {
            int current = _playerHealthManager.CurrentHealth;

            // Fill all hearts up to current health
            for (int i = 0; i < _hearts.Count; i++)
            {
                if (i < current)
                    _hearts[i].SetFull();
                else
                    _hearts[i].SetEmptyInstant();
            }

            // Animate damage (one heart at a time)
            if (current < _lastHealth)
            {
                CameraShake.Instance.Shake(0.5f);
                VignetteFlash.Instance.FlashInColor(Color.red);   // ← ADD THIS

                int heartsLost = _lastHealth - current;

                Sequence seq = DOTween.Sequence();

                for (int i = 0; i < heartsLost; i++)
                {
                    int heartIndex = _lastHealth - 1 - i;
                    seq.Append(_hearts[heartIndex].AnimateLoseHeart());
                }
            }


            _lastHealth = current;
        }
    }
}