using Art.Ui;
using DG.Tweening;
using Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class TutorialUI : MonoBehaviour
    {
        // -----------------------------
        // Public pulse methods (clean)
        // -----------------------------
        public void PulseLeft()        => _leftPulse?.TryPulse(0f, 2f, 0.15f);
        public void PulseRight()       => _rightPulse?.TryPulse(0f, 2f, 0.15f);
        public void PulseAttack()      => _attackPulse?.TryPulse(0f, 2f, 0.15f);
        public void PulseJump()        => _jumpPulse?.TryPulse(0f, 2f, 0.15f);
        public void PulseStateSwitch() => _stateSwitchPulse?.TryPulse(0f, 2f, 0.15f);

        public void PulseUpAttackGroup()
        {
            _upAttackPulse1?.TryPulse(0f, 2f, 0.15f);
            _upAttackPulse2?.TryPulse(0f, 2f, 0.15f);
            _upAttackPulse3?.TryPulse(0f, 2f, 0.15f);
        }

        // -----------------------------
        // Panels
        // -----------------------------
        [Header("Panels (PNG overlays)")]
        [SerializeField] private GameObject _moveJumpPanel;
        [SerializeField] private GameObject _attackPanel;
        [SerializeField] private GameObject _stateSwitchPanel;

        // -----------------------------
        // Pulse components
        // -----------------------------
        [Header("Pulse settings")]
        [SerializeField] private EmissionPulse _leftPulse;
        [SerializeField] private EmissionPulse _rightPulse;
        [SerializeField] private EmissionPulse _attackPulse;
        [SerializeField] private EmissionPulse _jumpPulse;
        [SerializeField] private EmissionPulse _stateSwitchPulse;
        [SerializeField] private EmissionPulse _upAttackPulse1;
        [SerializeField] private EmissionPulse _upAttackPulse2;
        [SerializeField] private EmissionPulse _upAttackPulse3;

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

        // ---------------------------------------------------------
        // Pulse → Shrink → Fade (all child images)
        // ---------------------------------------------------------
        public Tween PulseCurrentPanel()
        {
            GameObject panel = null;

            if (_moveJumpPanel != null && _moveJumpPanel.activeSelf)
                panel = _moveJumpPanel;
            else if (_attackPanel != null && _attackPanel.activeSelf)
                panel = _attackPanel;
            else if (_stateSwitchPanel != null && _stateSwitchPanel.activeSelf)
                panel = _stateSwitchPanel;

            if (panel == null)
                return null;

            // Get ALL images under the panel
            Image[] images = panel.GetComponentsInChildren<Image>(true);
            if (images.Length == 0)
                return null;

            // Reset visuals before animating
            ResetPanelVisuals(panel, images);

            Sequence seq = DOTween.Sequence().SetUpdate(true);

            // 1. Pulse (scale up)
            seq.Append(UiMovementUtility.Pulse(panel.transform, 1.2f, 0.15f));

            // 2. Shrink (scale down)
            seq.Append(UiMovementUtility.ShrinkTo(panel.transform, 0.15f, 0.8f));

            // 3. Fade ALL images
            foreach (var img in images)
                seq.Join(UiMovementUtility.Fade(img, 0.25f));

            return seq;
        }

        public Tween SpawnPanel(GameObject panel)
        {
            if (panel == null)
                return null;

            Image[] images = panel.GetComponentsInChildren<Image>(true);
            if (images.Length == 0)
                return null;

            // Start invisible & small
            foreach (var img in images)
            {
                var c = img.color;
                c.a = 0f;
                img.color = c;
            }

            panel.transform.localScale = Vector3.one * 0.7f;

            Sequence seq = DOTween.Sequence().SetUpdate(true);

            // 1. Fade in all images (JOIN)
            foreach (var img in images)
                seq.Join(img.DOFade(1f, 0.3f));

            // 2. Scale up at the SAME TIME (JOIN, not Append)
            seq.Join(panel.transform.DOScale(1.15f, 0.25f).SetEase(Ease.OutQuad));

            // 3. Settle pulse
            seq.Append(panel.transform.DOScale(1f, 0.1f));

            return seq;
        }

        private void ResetPanelVisuals(GameObject panel, Image[] images)
        {
            // Reset scale
            panel.transform.localScale = Vector3.one;

            // Reset alpha on ALL images
            foreach (var img in images)
            {
                var c = img.color;
                c.a = 1f;
                img.color = c;
            }
        }

        // ---------------------------------------------------------
        // Panel show/hide
        // ---------------------------------------------------------
        public void HideAll()
        {
            if (_moveJumpPanel) _moveJumpPanel.SetActive(false);
            if (_attackPanel) _attackPanel.SetActive(false);
            if (_stateSwitchPanel) _stateSwitchPanel.SetActive(false);

            _pulsing = false;
        }

        public Tween ShowMoveJump()
        {
            HideAll();
            if (_moveJumpPanel)
            {
                _moveJumpPanel.SetActive(true);
                return SpawnPanel(_moveJumpPanel);
            }
            return null;
        }

        public Tween ShowAttack()
        {
            HideAll();
            if (_attackPanel)
            {
                _attackPanel.SetActive(true);
                return SpawnPanel(_attackPanel);
            }
            return null;
        }

        public Tween ShowStateSwitch(bool pulse, Transform player)
        {
            HideAll();

            if (_stateSwitchPanel)
            {
                _stateSwitchPanel.SetActive(true);
                PositionStateSwitchOverPlayer(player);
                var t = SpawnPanel(_stateSwitchPanel);
                _pulsing = pulse;
                return t;
            }

            _pulsing = pulse;
            return null;
        }

        // ---------------------------------------------------------
        // Positioning
        // ---------------------------------------------------------
        private void PositionStateSwitchOverPlayer(Transform player)
        {
            if (_stateSwitchRect == null || player == null || _canvas == null)
                return;

            Vector3 worldPos = player.position;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            screenPos.y += _yOffset;

            if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                _stateSwitchRect.position = screenPos;
            }
            else if (_canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvas.transform as RectTransform,
                    screenPos,
                    _canvas.worldCamera,
                    out Vector2 localPos
                );

                _stateSwitchRect.localPosition = localPos;
            }
            else
            {
                Vector3 worldUIPos = Camera.main.ScreenToWorldPoint(screenPos);
                worldUIPos.z = _stateSwitchRect.position.z;
                _stateSwitchRect.position = worldUIPos;
            }
        }

        // ---------------------------------------------------------
        // Looping pulse for state switch
        // ---------------------------------------------------------
        private void Update()
        {
            if (!_pulsing || _stateSwitchRect == null)
                return;

            float s = 1f + Mathf.Sin(Time.unscaledTime * _pulseSpeed) * (_pulseScale - 1f);
            _stateSwitchRect.localScale = new Vector3(s, s, 1f);
        }
    }
}
