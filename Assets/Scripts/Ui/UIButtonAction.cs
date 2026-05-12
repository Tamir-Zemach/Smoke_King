using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ui
{
    public class UIButtonAction : MonoBehaviour
    {
        // Loads a scene by name
        public void Play(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        // Quits the application
        public void Exit()
        {
            Application.Quit();

            // This line helps when testing inside the editor
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}