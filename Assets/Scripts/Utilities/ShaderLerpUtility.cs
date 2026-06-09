using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class ShaderLerpUtility
    {
        private class CoroutineRunner : MonoBehaviour { }
        private static CoroutineRunner _runner;

        private static CoroutineRunner Runner
        {
            get
            {
                if (_runner != null) return _runner;
                GameObject go = new GameObject("ShaderLerpUtilityRunner");
                Object.DontDestroyOnLoad(go);
                _runner = go.AddComponent<CoroutineRunner>();
                return _runner;
            }
        }

        // ---------------------------------------------------------
        // SCALED TIME (original)
        // ---------------------------------------------------------
        public static Coroutine LerpFloat(Material mat, string property, float target, float duration)
        {
            return Runner.StartCoroutine(LerpFloatRoutine(mat, property, target, duration));
        }

        private static IEnumerator LerpFloatRoutine(Material mat, string property, float target, float duration)
        {
            float start = mat.GetFloat(property);
            float t = 0f;

            while (t < duration)
            {
                t += Time.deltaTime; // scaled
                float lerp = Mathf.Lerp(start, target, t / duration);
                mat.SetFloat(property, lerp);
                yield return null;
            }

            mat.SetFloat(property, target);
        }

        // ---------------------------------------------------------
        // UNSCALED TIME (new)
        // ---------------------------------------------------------
        public static Coroutine LerpFloatUnscaled(Material mat, string property, float target, float duration)
        {
            return Runner.StartCoroutine(LerpFloatRoutineUnscaled(mat, property, target, duration));
        }

        private static IEnumerator LerpFloatRoutineUnscaled(Material mat, string property, float target, float duration)
        {
            float start = mat.GetFloat(property);
            float t = 0f;

            while (t < duration)
            {
                t += Time.unscaledDeltaTime; // unscaled
                float lerp = Mathf.Lerp(start, target, t / duration);
                mat.SetFloat(property, lerp);
                yield return null;
            }

            mat.SetFloat(property, target);
        }
    }
}
