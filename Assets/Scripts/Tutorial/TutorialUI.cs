using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class TutorialUI : MonoBehaviour
    {
        [Header("Panels (PNG overlays)")]
        [SerializeField] private GameObject _moveJumpPanel;
        [SerializeField] private GameObject _attackPanel;
        [SerializeField] private GameObject _stateSwitchPanel;

        [Header("Pulse settings")]
        [SerializeField] private float _pulseScale = 1.1f;
        [SerializeField] private float _pulseSpeed = 1.5f;

        [Header("Positioning")]
        [SerializeField] private float _yOffset = 100f;

        private RectTransform _stateSwitchRect;
        private Canvas _canvas;
        private bool _pulsing;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();

            if (_stateSwitchPanel != null)
                _stateSwitchRect = _stateSwitchPanel.GetComponent<RectTransform>();

            HideAll();
        }

        public void HideAll()
        {
            if (_moveJumpPanel) _moveJumpPanel.SetActive(false);
            if (_attackPanel) _attackPanel.SetActive(false);
            if (_stateSwitchPanel) _stateSwitchPanel.SetActive(false);
            _pulsing = false;
        }

        public void ShowMoveJump()
        {
            HideAll();
            if (_moveJumpPanel) _moveJumpPanel.SetActive(true);
        }

        public void ShowAttack()
        {
            HideAll();
            if (_attackPanel) _attackPanel.SetActive(true);
        }

        public void ShowStateSwitch(bool pulse, Transform player)
        {
            HideAll();

            if (_stateSwitchPanel)
            {
                _stateSwitchPanel.SetActive(true);
                PositionStateSwitchOverPlayer(player);
            }

            _pulsing = pulse;
        }

        private void PositionStateSwitchOverPlayer(Transform player)
        {
            if (_stateSwitchRect == null || player == null || _canvas == null)
                return;

            Vector3 worldPos = player.position;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            screenPos.y += _yOffset;

            if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // Direct screen position
                _stateSwitchRect.position = screenPos;
            }
            else if (_canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                // Convert screen → canvas local
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvas.transform as RectTransform,
                    screenPos,
                    _canvas.worldCamera,
                    out Vector2 localPos
                );

                _stateSwitchRect.localPosition = localPos;
            }
            else // World Space Canvas
            {
                // Convert screen → world
                Vector3 worldUIPos = Camera.main.ScreenToWorldPoint(screenPos);
                worldUIPos.z = _stateSwitchRect.position.z;
                _stateSwitchRect.position = worldUIPos;
            }
        }

        private void Update()
        {
            if (!_pulsing || _stateSwitchRect == null)
                return;

            float s = 1f + Mathf.Sin(Time.unscaledTime * _pulseSpeed) * (_pulseScale - 1f);
            _stateSwitchRect.localScale = new Vector3(s, s, 1f);
        }
    }
}
