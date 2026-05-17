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

        private RectTransform _stateSwitchRect;
        private bool _pulsing;

        private void Awake()
        {
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

        public void ShowStateSwitch(bool pulse)
        {
            HideAll();
            if (_stateSwitchPanel) _stateSwitchPanel.SetActive(true);
            _pulsing = pulse;
        }

        private void Update()
        {
            if (!_pulsing || _stateSwitchRect == null) return;

            float s = 1f + Mathf.Sin(Time.unscaledTime * _pulseSpeed) * (_pulseScale - 1f);
            _stateSwitchRect.localScale = new Vector3(s, s, 1f);
        }
    }
}