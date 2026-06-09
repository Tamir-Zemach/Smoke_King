using System;
using Art.Ui;
using Audio;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Enums;
using UnityEngine.Events;

namespace Ui
{
    public class ButtonCustomBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        public float ClickScale = 1.2f;
        public float ClickDuration = 0.2f;
        public Ease ClickEase = Ease.OutBack;

        private Vector3 _originalScale;
        private Tween _scaleTween;

        void Awake()
        {
            _targetButton = GetComponent<Button>();
            _originalScale = transform.localScale;
            _pulse = GetComponent<EmissionPulse>();
            _cg = GetComponent<CanvasGroup>();


            if (_targetButton != null)
                _targetButton.onClick.AddListener(OnButtonClicked);
        }

        void OnEnable()
        {
            ResetStates();
        }

        private void ResetStates()
        {
            // Ensure components exist
            if (_pulse == null)
            {
                _pulse = GetComponent<EmissionPulse>();
                if (_pulse == null)
                {
                    _pulse = gameObject.AddComponent<EmissionPulse>();
                }
            }

            if (_cg == null)
            {
                _cg = GetComponent<CanvasGroup>();
                if (_cg == null)
                {
                    _cg = gameObject.AddComponent<CanvasGroup>();
                }
            }

            // Kill any running tween
            if (_scaleTween != null)
            {
                _scaleTween.Kill();
                _scaleTween = null;
            }

            // Reset scale
            transform.localScale = _originalScale;

            // Reset CanvasGroup
            _cg.blocksRaycasts = true;
            _cg.interactable = true;
            _cg.alpha = 1f;

            // Reset emission pulse intensity
            if (_pulse.TryGetComponent<Image>(out var img) && img.material != null)
                img.material.SetFloat("_EmissionIntensity", 0);
        }



        // -----------------------------
        // CLICK POP
        // -----------------------------
        void OnButtonClicked()
        {
            if (_scaleTween != null) _scaleTween.Kill();

            _cg.blocksRaycasts = false;
            _cg.interactable = false;
            AudioManager.Instance.PlaySfx(SfxType.Button);

            _scaleTween = transform
                .DOScale(_originalScale * ClickScale, ClickDuration)
                .SetEase(ClickEase)
                .SetUpdate(true) // <--- unscaled time
                .OnComplete(() =>
                {
                    _pulse.Pulse(_pulse.Min, _pulse.Max, _pulse.Duration);

                    _scaleTween = transform
                        .DOScale(_originalScale, ClickDuration)
                        .SetEase(ClickEase)
                        .SetUpdate(true)
                        .OnComplete(() =>
                        {
                            OnFinish?.Invoke();
                        });
                });
        }

        // -----------------------------
        // HOVER LOOP
        // -----------------------------
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_cg.blocksRaycasts) return;

            if (_scaleTween != null) _scaleTween.Kill();

            StartHoverLoop();
        }

        void StartHoverLoop()
        {
            transform.localScale = _originalScale * HoverScaleMin;

            _scaleTween = transform
                .DOScale(_originalScale * HoverScaleMax, ScaleDuration)
                .SetEase(ScaleEase)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true); // <--- unscaled time
        }

        // -----------------------------
        // EXIT
        // -----------------------------
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_cg.blocksRaycasts) return;

            if (_scaleTween != null) _scaleTween.Kill();

            _scaleTween = transform
                .DOScale(_originalScale, PointerExitScaleDuration)
                .SetEase(ScaleEase)
                .SetUpdate(true); // <--- unscaled time
        }


    }
}
