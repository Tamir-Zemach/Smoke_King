using Art.Ui;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

namespace Ui
{
    public class ButtonEmissionPulse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent OnFinish;
        private Button _targetButton;
        private EmissionPulse _pulse;
        private CanvasGroup _cg;

        [Header("Hover Scale Settings")]
        public float HoverScaleMin = 1.05f;
        public float HoverScaleMax = 1.1f;
        public float ScaleDuration = 0.3f;
        public float PointerExitScaleDuration = 0.15f;
        public Ease ScaleEase = Ease.InQuad;

        [Header("Click Pop Settings")]
        public float ClickScale = 1.15f;
        public float ClickDuration = 0.2f;
        public Ease ClickEase = Ease.OutBack;

        private Vector3 _originalScale;
        private Tween _scaleTween;

        void Awake()
        {
            _targetButton = GetComponent<Button>();
            _originalScale = transform.localScale;

            _pulse = GetComponent<EmissionPulse>();
            if (_pulse == null)
            {
                _pulse = gameObject.AddComponent<EmissionPulse>();
            }
            _cg = GetComponent<CanvasGroup>();
            if (_cg == null)
            {
                _cg = gameObject.AddComponent<CanvasGroup>();
            }

            if (_targetButton != null)
            {
                _targetButton.onClick.AddListener(OnButtonClicked);
            }
        }

        // -----------------------------
        // CLICK: pop to 1.25 then back
        // -----------------------------
        void OnButtonClicked()
        {
            if (_scaleTween != null) _scaleTween.Kill();
            
            _cg.blocksRaycasts = false;
            _cg.interactable = false;

            // Pop up
            _scaleTween = transform.DOScale(_originalScale * ClickScale, ClickDuration)
                .SetEase(ClickEase)
                .OnComplete(() =>
                {
                    _pulse.Pulse(_pulse.Min, _pulse.Max, _pulse.Duration);

                    // Return to original
                    _scaleTween = transform.DOScale(_originalScale, ClickDuration)
                        .SetEase(ClickEase)
                        .OnComplete(() =>
                        {
                            OnFinish?.Invoke();
                            
                        });
                });
        }

        // -----------------------------
        // HOVER LOOP: 1.05 -> 1.1 -> 1.05
        // -----------------------------
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_cg.blocksRaycasts) return; // hover blocked
            
            if (_scaleTween != null) _scaleTween.Kill();

            StartHoverLoop();
        }

        void StartHoverLoop()
        {
            transform.localScale = _originalScale * HoverScaleMin;

            _scaleTween = transform
                .DOScale(_originalScale * HoverScaleMax, ScaleDuration)
                .SetEase(ScaleEase)
                .SetLoops(-1, LoopType.Yoyo);
        }

        // -----------------------------
        // EXIT: return to original scale
        // -----------------------------
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_cg.blocksRaycasts) return; // ignore exit if blocked
            
            if (_scaleTween != null) _scaleTween.Kill();

            _scaleTween = transform
                .DOScale(_originalScale, PointerExitScaleDuration)
                .SetEase(ScaleEase);
        }
    }
}
