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
        
        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("StartingMenu");
        }
        
        public void PlaWithTransition(string sceneName)
        {
            StartCoroutine(PlayAndLoad(sceneName));
        }

        private IEnumerator PlayAndLoad(string sceneName)
        {
            // Start loading the scene in the background
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            bool finished = false;

            // Start your smoke transition
            SmokeTransition.PlayTransitionToRight(() =>
            {
                finished = true;
            });
            
            // Wait until smoke animation finishes
            yield return new WaitUntil(() => finished);

            // Scene is already loaded to 90–100% at this point
            asyncLoad.allowSceneActivation = true;
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