using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameOverEntrance : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light2D _light;
    [SerializeField] private float _startRadius = 0.1f;
    [SerializeField] private float _endRadius = 8f;
    [SerializeField] private float _growDuration = 1.2f;
    [SerializeField] private Ease _growEase = Ease.OutQuad;

    [Header("Canvas Fade")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeDelay = 0.2f;
    [SerializeField] private float _fadeDuration = 1f;

    [Header("Lights To Spawn")]
    [SerializeField] private GameObject[] _lightsToActivate;

    private Tween _radiusTween;

    private void Awake()
    {
        if (_light == null)
            _light = GetComponent<Light2D>();

        if (_canvasGroup != null)
            _canvasGroup.alpha = 0f;

        _light.pointLightOuterRadius = _startRadius;

        // Disable lights until the moment of reveal
        foreach (var obj in _lightsToActivate)
            if (obj != null)
                obj.SetActive(false);
    }

    private void OnEnable()
    {
        StartEntrance();
    }

    public void StartEntrance()
    {
        _radiusTween?.Kill();

        _radiusTween = DOTween.To(
                () => _light.pointLightOuterRadius,
                r => _light.pointLightOuterRadius = r,
                _endRadius,
                _growDuration
            )
            .SetEase(_growEase)
            .OnComplete(OnLightFullyOpened);
    }

    private void OnLightFullyOpened()
    {
        // Activate lights
        foreach (var obj in _lightsToActivate)
            if (obj != null)
                obj.SetActive(true);

        // Fade in UI
        if (_canvasGroup != null)
        {
            _canvasGroup
                .DOFade(1f, _fadeDuration)
                .SetDelay(_fadeDelay)
                .SetEase(Ease.OutQuad);
        }
    }
}
