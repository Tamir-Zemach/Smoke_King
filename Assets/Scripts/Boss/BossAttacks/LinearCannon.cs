using System;
using System.Collections;
using Data;
using Enums;
using ObjectPooling;
using Particles;
using Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utilities;

namespace Boss.BossAttacks
{
    public class LinearCannon : MonoBehaviour
    {
        [Header("General")]
        public bool Tracking;

        public TrackingCannonData TrackingCannonData;
        public CannonAttackData LinearCannonData;

        [Header("References")]
        public SmokeParticleManager SmokeParticle; // Holds ParticleSystem + ParticleMovementUtility + ParticleDamage2D


        private Action _onFinish;
        private PlayerHealthManager _player;
        private ObjectPool<LinearCannon> _pool;
        private StateType _state;

        private CannonAttackData ActiveData => Tracking ? TrackingCannonData : LinearCannonData;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.up * ActiveData.MaxDistance);
        }

        public void Init(Vector3 position, Quaternion rotation, Action onFinished = null)
        {
            _onFinish = onFinished;

            transform.position = position;
            transform.rotation = rotation;

            if (Tracking)
            {
                _player = FindAnyObjectByType<PlayerHealthManager>();
            }

            SetRandomState();
            ResetStates();

            StartCoroutine(FireRoutine());
        }



        private void SetRandomState()
        {
            _state = EnumUtility.GetRandomValue<StateType>();
            var visual = ActiveData.GetVisual(_state);
            SmokeParticle.Init(visual.Color, visual.Material, visual.Type);
        }


        private void ResetStates()
        {
            SmokeParticle.ResetPosition(transform.position);
        }

        private IEnumerator FireRoutine()
        {

            // TRACKING PHASE
            if (Tracking)
            {
                yield return StartCoroutine(TrackingRoutine());
                
            }
            else
            {
                SmokeParticle.MoveInCircle(ActiveData.DelayBeforeFire);
                yield return new WaitForSeconds(ActiveData.DelayBeforeFire);
            }

            // MoveInCircle → Fly (particles emit during movement)
            SmokeParticle.Fly();

            // BeamDuration = how long the beam exists before returning to pool
            yield return new WaitForSeconds(ActiveData.BeamDuration);

            _onFinish?.Invoke();
        }

        private IEnumerator TrackingRoutine()
        {
            var data = ActiveData as TrackingCannonData;

            if (data == null)
            {
                Debug.LogError("Tracking is enabled but TrackingCannonData is missing!");
                yield break;
            }

            float timer = 0f;

            while (timer < data.TrackingDuration)
            {
                timer += Time.deltaTime;

                if (_player != null)
                {
                    var dir = (_player.transform.position - transform.position).normalized;
                    var targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

                    var angle = Mathf.MoveTowardsAngle(
                        transform.eulerAngles.z,
                        targetAngle,
                        data.RotationSpeed * Time.deltaTime
                    );

                    transform.rotation = Quaternion.Euler(0, 0, angle);
                }

                yield return null;
            }

            if (data.LockDuration > 0f)
                yield return new WaitForSeconds(data.LockDuration);
        }
    }
}
