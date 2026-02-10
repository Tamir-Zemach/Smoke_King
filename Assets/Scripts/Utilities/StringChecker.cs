using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities
{
    public static class StringChecker
    {
        // -------------------------
        // Animator Trigger Checker
        // -------------------------
        public static bool AnimatorHasString(Animator animatorToCheck, string triggerName)
        {
            if (HasTrigger(animatorToCheck, triggerName))
                return true;

            Debug.LogError($"Animator missing trigger: {triggerName}");
            return false;
        }

        private static bool HasTrigger(Animator animatorToCheck, string triggerName)
        {
            return animatorToCheck.parameters.Any(param => param.name == triggerName);
        }

        // -------------------------
        // Scene Existence Checker
        // -------------------------
        public static bool SceneExists(string sceneName)
        {
            var count = SceneManager.sceneCountInBuildSettings;

            for (var i = 0; i < count; i++)
            {
                var path = SceneUtility.GetScenePathByBuildIndex(i);
                var name = Path.GetFileNameWithoutExtension(path);

                if (name == sceneName)
                    return true;
            }

            Debug.LogError($"Scene '{sceneName}' does not exist in Build Settings.");
            return false;
        }
    }
}