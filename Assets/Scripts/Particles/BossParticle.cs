using System;
using System.Collections;
using UnityEngine;

namespace Particles
{
    public class BossParticle : MonoBehaviour
    {
        private ParticleSystem _ps;
        private Action<BossParticle> _onFinished;

        private void Awake()
        {
            _ps = GetComponent<ParticleSystem>();
        }

        public void SetStartColor(Color color)
        {
            var main = _ps.main;
            main.startColor = color;
        }

        public void PlayAt(Vector3 pos, Action<BossParticle> onFinished)
        {
            _onFinished = onFinished;

            transform.localPosition = pos;

            if (_ps == null)
                _ps = GetComponent<ParticleSystem>();

            _ps.Play();
            StartCoroutine(ReturnWhenDone());
        }

        private IEnumerator ReturnWhenDone()
        {
            yield return new WaitUntil(() => !_ps.IsAlive(true));
            _onFinished?.Invoke(this);
        }

        public void Stop()
        {
            _ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            _onFinished?.Invoke(this);
        }
    }
}