using UnityEngine;

namespace SoraKobo.UI
{
    /// <summary>
    /// Adapts camera orthographic size to maintain a consistent world view
    /// regardless of screen resolution or aspect ratio.
    /// Attach to the Main Camera.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class ResolutionAdapter : MonoBehaviour
    {
        [Header("Reference Resolution")]
        public float referenceHeight = 1080f;
        public float referenceOrthographicSize = 7f;

        [Header("Safe Area")]
        public RectTransform safeAreaPanel;

        private UnityEngine.Camera _cam;
        private Rect _lastSafeArea = Rect.zero;
        private int _lastScreenWidth;
        private int _lastScreenHeight;

        void Awake()
        {
            _cam = GetComponent<UnityEngine.Camera>();
            Refresh();
        }

        void Update()
        {
            // Only recalculate when screen size changes (device rotation, etc.)
            if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
                Refresh();

            if (Screen.safeArea != _lastSafeArea)
                ApplySafeArea();
        }

        void Refresh()
        {
            _lastScreenWidth  = Screen.width;
            _lastScreenHeight = Screen.height;

            if (_cam != null && _cam.orthographic)
            {
                float scale = Screen.height / referenceHeight;
                _cam.orthographicSize = referenceOrthographicSize / scale;
            }

            ApplySafeArea();
        }

        void ApplySafeArea()
        {
            _lastSafeArea = Screen.safeArea;
            if (safeAreaPanel == null) return;

            var parent = safeAreaPanel.parent as RectTransform;
            if (parent == null) return;

            Vector2 anchorMin = Screen.safeArea.position;
            Vector2 anchorMax = Screen.safeArea.position + Screen.safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            safeAreaPanel.anchorMin = anchorMin;
            safeAreaPanel.anchorMax = anchorMax;
        }
    }
}
