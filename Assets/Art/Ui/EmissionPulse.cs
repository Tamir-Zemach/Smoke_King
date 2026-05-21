using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Art.Ui
{
    [RequireComponent(typeof(Image))]
    public class EmissionPulse : MonoBehaviour
    {
        [Header("Debug Test Values")]
        public float TestMin = 0;
        public float TestMax = 2f;
        public float TestDuration = 1f;

        [Tooltip("Tick this in Play mode to fire a one-shot pulse using the test values above. Auto-resets.")]
        public bool TriggerPulse = false;

        static readonly int IntensityID = Shader.PropertyToID("_EmissionIntensity");

        Image _image;
        Material _instance;
        Coroutine _current;

        void Awake()
        {
            _image = GetComponent<Image>();
            if (_image.material == null) return;

            _instance = new Material(_image.material);
            _image.material = _instance;
        }

        void Update()
        {
            if (TriggerPulse)
            {
                TriggerPulse = false;
                Pulse(TestMin, TestMax, TestDuration);
            }
        }

        // ---------------------------------------------------------
        // CLEANEST: Only pulse if current emission is below threshold
        // ---------------------------------------------------------
        public void TryPulse(float min, float max, float dur, float threshold = 0.5f)
        {
            if (_instance == null) return;

            float current = _instance.GetFloat(IntensityID);

            // Already emissive enough → skip
            if (current > threshold)
                return;

            Pulse(min, max, dur);
        }

        public void Pulse(float min, float max, float dur)
        {
            if (_instance == null) return;

            if (_current != null)
                CoroutineRunner.Instance.StopCoroutine(_current);

            _current = CoroutineRunner.Instance.StartCoroutine(PulseRoutine(min, max, dur));
        }

        IEnumerator PulseRoutine(float min, float max, float dur)
        {
            if (dur <= 0f)
            {
                _instance.SetFloat(IntensityID, max);
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < dur)
            {
                float t = elapsed / dur;
                float v = Mathf.Lerp(min, max, t);
                _instance.SetFloat(IntensityID, v);

                elapsed += Time.unscaledDeltaTime; // << FIX
                yield return null;
            }

            _instance.SetFloat(IntensityID, max);
            _current = null;
        }


        [ContextMenu("Test Pulse")]
        void DebugPulse() => Pulse(TestMin, TestMax, TestDuration);

        void OnDestroy()
        {
            if (_instance != null) Destroy(_instance);
        }
    }
}
