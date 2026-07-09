using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace SoraKobo.UI
{
    /// <summary>
    /// Shows brief toast notifications (e.g. "Map saved!", "Player joined").
    /// Place a single instance on the HUD Canvas.
    /// </summary>
    public class ToastNotification : MonoBehaviour
    {
        public static ToastNotification Instance { get; private set; }

        [Header("UI")]
        public CanvasGroup canvasGroup;
        public TextMeshProUGUI messageText;
        public float displayDuration = 2f;
        public float fadeDuration    = 0.3f;

        private Coroutine _current;

        void Awake()
        {
            Instance = this;
            if (canvasGroup != null) canvasGroup.alpha = 0f;
        }

        public static void Show(string message)
        {
            Instance?.ShowToast(message);
        }

        public void ShowToast(string message)
        {
            if (_current != null) StopCoroutine(_current);
            _current = StartCoroutine(ToastRoutine(message));
        }

        IEnumerator ToastRoutine(string message)
        {
            if (messageText != null) messageText.text = message;

            // Fade in
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                if (canvasGroup != null) canvasGroup.alpha = t / fadeDuration;
                yield return null;
            }
            if (canvasGroup != null) canvasGroup.alpha = 1f;

            yield return new WaitForSeconds(displayDuration);

            // Fade out
            t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                if (canvasGroup != null) canvasGroup.alpha = 1f - t / fadeDuration;
                yield return null;
            }
            if (canvasGroup != null) canvasGroup.alpha = 0f;
        }
    }
}
