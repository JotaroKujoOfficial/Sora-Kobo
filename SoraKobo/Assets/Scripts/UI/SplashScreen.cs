using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace SoraKobo.UI
{
    public class SplashScreen : MonoBehaviour
    {
        [Header("UI")]
        public CanvasGroup canvasGroup;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI subtitleText;
        public float displayDuration = 2.5f;
        public float fadeDuration    = 0.8f;

        void Start()
        {
            if (titleText    != null) titleText.text    = "Sora Kobo";
            if (subtitleText != null) subtitleText.text = "空の工房";
            StartCoroutine(SplashRoutine());
        }

        IEnumerator SplashRoutine()
        {
            if (canvasGroup != null) canvasGroup.alpha = 0f;
            // Fade in
            yield return Fade(0f, 1f, fadeDuration);
            // Hold
            yield return new WaitForSeconds(displayDuration);
            // Fade out
            yield return Fade(1f, 0f, fadeDuration);
            // Go to main menu
            gameObject.SetActive(false);
            UIManager.Instance?.ShowScreen("MainMenu");
        }

        IEnumerator Fade(float from, float to, float dur)
        {
            float t = 0f;
            while (t < dur)
            {
                t += Time.deltaTime;
                if (canvasGroup != null) canvasGroup.alpha = Mathf.Lerp(from, to, t / dur);
                yield return null;
            }
            if (canvasGroup != null) canvasGroup.alpha = to;
        }
    }
}
