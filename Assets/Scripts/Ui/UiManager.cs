using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Ui
{
    public class UiManager : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            if (StringChecker.SceneExists(sceneName)) SceneManager.LoadScene(sceneName);
        }
    }
}