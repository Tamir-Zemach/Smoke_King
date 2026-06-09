using System;
using Audio;
using Boss;
using Player;
using Ui;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Managers
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [SerializeField] private PlayerHealthManager _playerHealthManager;
        [SerializeField] private BossHealthManager _bossHealthManager;
        [SerializeField] private UiManager _uiManager;
            
            
        public Action OnGameOver;
        public Action OnWinGame;
        public bool GameIsPaused { get; private set; }

        private void Start()
        {
            Subscribe();
            GameIsPaused = false;
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
            _bossHealthManager.OnBossDied += WinGame;
        }


        private void GameOver()
        {
            OnGameOver?.Invoke();
        }

        private void WinGame()
        {
            OnWinGame?.Invoke();
            GameIsPaused = false;
        }

        private void Update()
        {
            if (_playerHealthManager.IsInDeathSequence)
            {
                GameIsPaused = false;
                AudioListener.pause = false;
                return;
            }

            GameIsPaused = _uiManager.Paused;
            AudioListener.pause = GameIsPaused;
        }



    }
}