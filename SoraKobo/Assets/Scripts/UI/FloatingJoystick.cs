using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoraKobo.UI
{
    public class FloatingJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Joystick Settings")]
        public float handleRange = 80f;
        public float deadZone = 0.1f;
        public RectTransform background;
        public RectTransform handle;

        private Canvas _canvas;
        private Camera _cam;
        private RectTransform _rect;
        private Vector2 _input;
        private Vector2 _bgOrigPos;

        public float Horizontal => Mathf.Abs(_input.x) > deadZone ? _input.x : 0f;
        public float Vertical   => Mathf.Abs(_input.y) > deadZone ? _input.y : 0f;
        public Vector2 Direction => new Vector2(Horizontal, Vertical);
        public bool IsPressed { get; private set; }

        void Start()
        {
            _canvas  = GetComponentInParent<Canvas>();
            _rect    = GetComponent<RectTransform>();
            _cam     = _canvas?.renderMode == RenderMode.ScreenSpaceCamera ? _canvas.worldCamera : null;

            if (background != null)
            {
                _bgOrigPos = background.anchoredPosition;
                background.gameObject.SetActive(false);
            }
            if (handle != null)
                handle.anchoredPosition = Vector2.zero;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsPressed = true;

            if (background != null)
            {
                background.gameObject.SetActive(true);
                background.position = eventData.position;
            }

            if (handle != null)
                handle.anchoredPosition = Vector2.zero;

            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background, eventData.position, _cam, out pos);

            pos /= handleRange;
            _input = Vector2.ClampMagnitude(pos, 1f);

            if (handle != null)
                handle.anchoredPosition = _input * handleRange;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsPressed = false;
            _input = Vector2.zero;

            if (background != null)
                background.gameObject.SetActive(false);

            if (handle != null)
                handle.anchoredPosition = Vector2.zero;
        }
    }
}
