using System;
using System.Collections;
using Enums;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Particles
{
    public class ImpactParticle : MonoBehaviour
    {
        private ParticleSystem _ps;
        private ParticleSystemRenderer _parRenderer;
        private Light2D _light;

        private Action<ImpactParticle> _onFinished;

        private void Awake()
        {
            _ps = GetComponent<ParticleSystem>();
            _parRenderer = GetComponent<ParticleSystemRenderer>();
            _light = GetComponentInChildren<Light2D>();

        }

        public void PlayAt(Vector3 position,Material material, Color color,Action<ImpactParticle> onFinished)
        {
            _onFinished = onFinished;

            transform.position = position;
            _parRenderer.material = material;
            _light.color = color;
            gameObject.SetActive(true);
            _ps.Play();
            
            StartCoroutine(ReturnWhenDone());
        }

        private IEnumerator ReturnWhenDone()
        {
            yield return new WaitUntil(() => !_ps.IsAlive(true));
            _onFinished?.Invoke(this);
        }
    }
}