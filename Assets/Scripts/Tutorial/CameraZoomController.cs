using Unity.Cinemachine;
using UnityEngine;

namespace Tutorial
{
    public class CameraZoomController : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera _vcam;
        [SerializeField] private float _zoomedSize = 3f;
        [SerializeField] private float _zoomSpeed = 5f;

        private float _defaultSize;
        private float _targetSize;

        private void Awake()
        {
            if (_vcam == null)
                _vcam = GetComponent<CinemachineCamera>();

            if (_vcam != null)
            {
                _defaultSize = _vcam.Lens.OrthographicSize;
                _targetSize = _defaultSize;
            }
        }

        private void Update()
        {
            if (_vcam == null) return;

            float size = Mathf.Lerp(_vcam.Lens.OrthographicSize, _targetSize, Time.unscaledDeltaTime * _zoomSpeed);
            _vcam.Lens.OrthographicSize = size;
        }

        public void ZoomIn()
        {
            _targetSize = _zoomedSize;
        }

        public void ZoomOut()
        {
            _targetSize = _defaultSize;
        }
    }
}