using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace SoraKobo.UI
{
    /// <summary>
    /// Adds satisfying press feedback to UI buttons on mobile.
    /// Attach to any Button for a scale-bounce effect on tap.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class MobileButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Animation")]
        public float pressScale   = 0.90f;
        public float releaseScale = 1.05f;
        public float duration     = 0.08f;

        private Vector3 _originalScale;
        private Coroutine _anim;

        void Start() => _originalScale = transform.localScale;

        public void OnPointerDown(PointerEventData e)
        {
            if (_anim != null) StopCoroutine(_anim);
            _anim = StartCoroutine(ScaleTo(_originalScale * pressScale, duration));
        }

        public void OnPointerUp(PointerEventData e)
        {
            if (_anim != null) StopCoroutine(_anim);
            _anim = StartCoroutine(BounceBack());
        }

        IEnumerator ScaleTo(Vector3 target, float dur)
        {
            Vector3 start = transform.localScale;
            float t = 0f;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                transform.localScale = Vector3.Lerp(start, target, t / dur);
                yield return null;
            }
            transform.localScale = target;
        }

        IEnumerator BounceBack()
        {
            yield return ScaleTo(_originalScale * releaseScale, duration);
            yield return ScaleTo(_originalScale,                duration * 0.5f);
        }

        void OnDisable() => transform.localScale = _originalScale;
    }
}
