using UnityEngine.InputSystem; // <-- required
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Ui
{
    public class CursorController : SingletonMonoBehaviour<CursorController>
    {
        [Header("Cursor UI")]
        [SerializeField] private RectTransform _cursorUI;
        private Canvas _cursorCanvas;

        [Header("Scaling")]
        [SerializeField] private float _baseScale = 1f; 
        [SerializeField] private float _referenceHeight = 1080f;

        [Header("Hotspot Offset")]
        [SerializeField] private Vector2 _hotspotOffset = new Vector2(-25, 32);

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            _cursorCanvas = GetComponent<Canvas>();
            Cursor.visible = false; 
        }

        private void Update()
        {
            UpdateCursorPosition();
            UpdateCursorScale();
        }
        private bool _isVisible = true;

        public void ShowCursor()
        {
            _isVisible = true;
            _cursorUI.gameObject.SetActive(true);
            Cursor.visible = false;
        }

        public void HideCursor()
        {
            _isVisible = false;
            _cursorUI.gameObject.SetActive(false);
            Cursor.visible = false;
        }

        public bool IsCursorVisible()
        {
            return _isVisible;
        }





        private void UpdateCursorPosition()
        {
            if (Mouse.current == null)
                return; // no mouse available (e.g., controller-only platforms)

            Vector2 mousePos = Mouse.current.position.ReadValue();

            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _cursorCanvas.transform as RectTransform,
                mousePos,
                _cursorCanvas.worldCamera,
                out pos
            );

            _cursorUI.anchoredPosition = pos + _hotspotOffset;
        }

        private void UpdateCursorScale()
        {
            float scaleFactor = Screen.height / _referenceHeight;
            _cursorUI.localScale = Vector3.one * scaleFactor * _baseScale;
        }

        /// <summary>
        /// Change the cursor sprite at runtime.
        /// </summary>
        public void SetCursorSprite(Sprite sprite)
        {
            _cursorUI.GetComponent<Image>().sprite = sprite;
        }

        /// <summary>
        /// Change hotspot offset (e.g., tip of spear).
        /// </summary>
        public void SetHotspot(Vector2 offset)
        {
            _hotspotOffset = offset;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_cursorUI == null)
                return;

            // Convert the anchored hotspot position into world space
            Vector3 worldPos = _cursorUI.transform.TransformPoint(_hotspotOffset);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(worldPos, 5f); // small red dot

            // Optional: draw a crosshair for clarity
            float size = 10f;
            Gizmos.DrawLine(worldPos + Vector3.left * size, worldPos + Vector3.right * size);
            Gizmos.DrawLine(worldPos + Vector3.up * size, worldPos + Vector3.down * size);
        }
#endif

    }
    
}