using System;
using System.Collections;
using Data;
using Enums;
using ObjectPooling;
using Particles;
using Player;
using Structs;
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

        public TrackingSmokeParticleManager TrackingSmokeParticleSmokeParticle;


        private Action _onFinish;
        private PlayerHealthManager _player;
        private ObjectPool<LinearCannon> _pool;
        private StateType _state;
        private VisualData _visualData;

        private CannonAttackData ActiveData => Tracking ? TrackingCannonData : LinearCannonData;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.up * ActiveData.MaxDistance);
        }

        public void Init(Vector3 position, Quaternion rotation, StateType state,  Action onFinished = null)
        { 
            _onFinish = onFinished;
            _state = state;
            transform.position = position;
            transform.rotation = rotation;
            _visualData = ActiveData.GetVisual(_state);
            if (Tracking)
            {
                _player = FindAnyObjectByType<PlayerHealthManager>();
                TrackingSmokeParticleSmokeParticle.Init(_visualData.Material);
            }
            
            SmokeParticle.Init(_visualData.Color, _visualData.Material, _visualData.Type);


            StartCoroutine(FireRoutine());
        }


        


        private void ResetStates()
        {
            SmokeParticle.ResetPos(transform.position);
            if (Tracking)
            {
                TrackingSmokeParticleSmokeParticle.ResetPos(transform.position);
            }
            
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
            ResetStates();
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

            SmokeParticle.MoveInCircle(data.TrackingDuration);
            TrackingSmokeParticleSmokeParticle.SetDuration(data.TrackingDuration);
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
