using System;
using Boss;
using Player;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [SerializeField] private PlayerHealthManager _playerHealthManager;
        [SerializeField] private BossHealthManager _bossHealthManager;
        public Action OnGameOver;
        public Action OnWinGame;

        private void Start()
        {
            Subscribe();
        }

        private void Subscribe(bool unsubscribe = false)
        {
            if (unsubscribe)
            {
                _playerHealthManager.OnDying -= GameOver;
                _bossHealthManager.OnBossDied -= WinGame;
                return;
            }

            _playerHealthManager.OnDying += GameOver;
            _bossHealthManager.OnBossDied -= WinGame;
        }


        private void GameOver()
        {
            OnGameOver?.Invoke();
        }

        private void WinGame()
        {
            OnWinGame?.Invoke();
        }
    }
}