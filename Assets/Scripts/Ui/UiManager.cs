
using Interfaces;
using Managers;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Ui
{
    public class UiManager : MonoBehaviour, IInputGetter
    {
        public PlayerInput Input { get; set; }
        
        //TODO: need to change!!!
        public bool Paused { get; private set; }
        
        [SerializeField] private GameObject _pausePanel;
        private void Awake()
        {
            Input = FindAnyObjectByType<PlayerInput>();
        }
        
        private void Start()
        {
            Input.OnPause += TogglePause;
            GameManager.Instance.OnGameOver += GameOverUi;
        }

        private void GameOverUi()
        {
            CursorController.Instance.ShowCursor();
            SceneManager.LoadScene("GameOver");
        }

        private void OnDestroy()
        {
            Input.OnPause -= TogglePause;
            GameManager.Instance.OnGameOver -= GameOverUi;
        }

        public void TogglePause()
        {
            Paused = !Paused;

            if (Paused)
            {
                Time.timeScale = 0f;
                _pausePanel.SetActive(true);
                CursorController.Instance.ShowCursor();

            }
            else
            {
                Time.timeScale = 1f;
                _pausePanel.SetActive(false);
                CursorController.Instance.HideCursor();
            }

        }
        

        

    }
}