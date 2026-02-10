using System;
using System.Collections;
using Data;
using Enums;
using ObjectPooling;
using Player;
using UnityEngine;
using Utilities;

namespace Boss.BossAttacks
{
    public class LinearCannon : MonoBehaviour
    {
        [Header("General")]
        public bool Tracking = false;
        private PlayerHealthManager _player;
        public TrackingCannonData TrackingCannonData;
        public CannonAttackData LinearCannonData;
        public Transform Beam; 
        public SpriteRenderer SpriteRendererIndex;

        private CannonAttackData ActiveData => Tracking ? TrackingCannonData : LinearCannonData;

        private Action _onFinish;
        private StateType _state;
        private Gradient _colorGradient;
        private LineRenderer _lineRenderer;
        private DamageGiver _damageGiver;

        private float _currentLength = 0f;
        private float _targetLength = 0f;
        private ObjectPool<LinearCannon> _pool;

        private void Awake()
        {
            _damageGiver = Beam.GetComponentInChildren<DamageGiver>();
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
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

            _damageGiver.StateType = _state;
            _colorGradient = visual.Gradient;
            SpriteRendererIndex.sprite = visual.IndexSprite;
            SpriteRendererIndex.color = visual.IndexSpriteColor;
        }

        private void ResetStates()
        {
            _currentLength = 0f;
            _targetLength = 0f;
            Beam.localScale = Vector3.zero;
            Beam.gameObject.SetActive(false);
            _lineRenderer.enabled = false;
            SpriteRendererIndex.gameObject.SetActive(true);
        }

        private IEnumerator FireRoutine()
        {
            SpriteRendererIndex.gameObject.SetActive(true);
            Beam.gameObject.SetActive(false);

            _lineRenderer.colorGradient = _colorGradient;
            _lineRenderer.enabled = false;

            // If tracking is enabled → rotate toward player first
            if (Tracking)
            {
                yield return StartCoroutine(TrackingRoutine());
            }
            else
            {
                yield return new WaitForSeconds(ActiveData.DelayBeforeFire);
            }

            // Fire phase
            ComputeTargetLength();
            SpriteRendererIndex.gameObject.SetActive(false);
            Beam.gameObject.SetActive(true);
            _lineRenderer.enabled = true;

            yield return StartCoroutine(AnimateBeam());
            yield return new WaitForSeconds(ActiveData.BeamDuration);

            _onFinish?.Invoke();
            
        }

        private IEnumerator TrackingRoutine()
        {
            TrackingCannonData data = ActiveData as TrackingCannonData;

            if (data == null)
            {
                Debug.LogError("Tracking is enabled but TrackingCannonData is missing!");
                yield break;
            }

            float timer = 0f;

            // 1. Rotate toward player
            while (timer < data.TrackingDuration)
            {
                timer += Time.deltaTime;

                if (_player != null)
                {
                    Vector3 dir = (_player.transform.position - transform.position).normalized;
                    float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

                    float angle = Mathf.MoveTowardsAngle(
                        transform.eulerAngles.z,
                        targetAngle,
                        data.RotationSpeed * Time.deltaTime
                    );

                    transform.rotation = Quaternion.Euler(0, 0, angle);
                }

                yield return null;
            }

            // 2. NEW: Hold the rotation for a moment before firing
            if (data.LockDuration > 0f)
                yield return new WaitForSeconds(data.LockDuration);
        }

        private void ComputeTargetLength()
        {
            var hit = Physics2D.Raycast(transform.position, transform.up, ActiveData.MaxDistance, ActiveData.WallLayer);
            _targetLength = hit.collider ? hit.distance : ActiveData.MaxDistance;
        }

        private IEnumerator AnimateBeam()
        {
            _currentLength = 0f;

            while (_currentLength < _targetLength)
            {
                _currentLength += ActiveData.BeamGrowSpeed * Time.deltaTime;
                _currentLength = Mathf.Min(_currentLength, _targetLength);

                Beam.localScale = new Vector3(1, _currentLength, 1);
                Beam.localPosition = new Vector3(0, _currentLength / 2f, 0);

                _lineRenderer.SetPosition(0, transform.position);
                _lineRenderer.SetPosition(1, transform.position + transform.up * _currentLength);

                yield return null;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.up * ActiveData.MaxDistance);
        }
    }
}