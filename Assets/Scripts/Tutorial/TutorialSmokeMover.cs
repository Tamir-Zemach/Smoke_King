using Cameras;
using UnityEngine;

namespace Tutorial
{
    public class TutorialSmokeMover : MonoBehaviour
    {
        private Transform _player;

        [Header("Movement Settings")]
        public float Speed = 4f;
        public float IdleDuration = 1.0f;
        public float ShakeIntensity = 0.15f;
        public float ShakeSpeed = 25f;
        public float CameraShakeIntensity = 1f;

        private bool _frozen = false;
        private bool _flying = false;
        private bool _cameraShakeStarted = false;

        private float _shakeTimer = 0f;
        private Vector3 _originalPos;

        private void Start()
        {
            _player = FindAnyObjectByType<Player.PlayerMovementManager>().transform;
            _originalPos = transform.position;
        }

        private void Update()
        {
            if (_frozen || _player == null)
                return;

            if (!_flying)
            {
                RunShakeMotion();
                return;
            }

            FlyTowardPlayer();
        }

        private void RunShakeMotion()
        {
            // Trigger camera shake ONCE
            if (!_cameraShakeStarted)
            {
                _cameraShakeStarted = true;
                CameraShake.Instance.Shake(CameraShakeIntensity, IdleDuration);
            }

            _shakeTimer += Time.deltaTime;

            float offsetX = Mathf.Sin(_shakeTimer * ShakeSpeed) * ShakeIntensity;
            float offsetY = Mathf.Cos(_shakeTimer * ShakeSpeed * 0.8f) * ShakeIntensity;

            transform.position = _originalPos + new Vector3(offsetX, offsetY, 0f);

            if (_shakeTimer >= IdleDuration)
            {
                _flying = true;
            }
        }

        private void FlyTowardPlayer()
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                _player.position,
                Speed * Time.deltaTime
            );
        }

        public void Freeze() => _frozen = true;
        public void Unfreeze() => _frozen = false;
    }
}
