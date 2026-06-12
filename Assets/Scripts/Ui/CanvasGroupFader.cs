using UnityEngine;
using DG.Tweening;

namespace Ui
{
    public class CanvasGroupFader : MonoBehaviour
    {
        [Header("Canvas Groups")]
        public CanvasGroup CanvasGroup1;
        public CanvasGroup CanvasGroup2;

        [Header("Settings")]
        public float FadeDuration = 0.3f;

        private bool _showingFirst = true;
        private void OnEnable()
        {
            CanvasGroup1.alpha = _showingFirst ? 1f : 0f;
            CanvasGroup1.interactable = _showingFirst;
            CanvasGroup1.blocksRaycasts = _showingFirst;

            CanvasGroup2.alpha = _showingFirst ? 0f : 1f;
            CanvasGroup2.interactable = !_showingFirst;
            CanvasGroup2.blocksRaycasts = !_showingFirst;
        }




        public void ToggleFade()
        {
            if (_showingFirst)
                Fade(CanvasGroup1, CanvasGroup2);
            else
                Fade(CanvasGroup2, CanvasGroup1);

            _showingFirst = !_showingFirst;
        }

        private void Fade(CanvasGroup from, CanvasGroup to)
        {
            // Fade out "from"
            from.DOFade(0f, FadeDuration)
                .SetUpdate(true); // <--- ignore timeScale
            from.interactable = false;
            from.blocksRaycasts = false;

            // Fade in "to"
            to.DOFade(1f, FadeDuration)
                .SetUpdate(true); // <--- ignore timeScale
            to.interactable = true;
            to.blocksRaycasts = true;
        }

    }
}