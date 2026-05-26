using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ui
{
    public class UIButtonAction : MonoBehaviour
    {
        public UiSmokeTransition SmokeTransition;
        // Loads a scene by name
        public void Play(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        
        public void PlaWithTransition(string sceneName)
        {
            StartCoroutine(PlayAndLoad(sceneName));
        }

        private IEnumerator PlayAndLoad(string sceneName)
        {
            bool finished = false;

            SmokeTransition.PlayTransitionToRight(() =>
            {
                finished = true;
            });

            // Wait until animation reports completion
            yield return new WaitUntil(() => finished);

            // Optional delay
            yield return new WaitForSeconds(1.1f);

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