using System;
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
            GameIsPaused = true;
        }

        private void WinGame()
        {
            OnWinGame?.Invoke();
            //SceneManager.LoadScene("GameWin");
            GameIsPaused = false;
        }

        private void Update()
        {
            //CHANGE!!!!!!!
            GameIsPaused = _uiManager.Paused;
        }


    }
}