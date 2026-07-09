using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace SoraKobo.UI
{
    public class LoadingScreen : MonoBehaviour
    {
        public static LoadingScreen Instance { get; private set; }

        [Header("UI")]
        public GameObject loadingPanel;
        public Slider progressBar;
        public TextMeshProUGUI statusText;
        public TextMeshProUGUI tipText;

        private static readonly string[] Tips =
        {
            "Tip: Place blocks while holding the build button to paint quickly!",
            "Tip: You can sit on chairs and play the piano!",
            "Tip: Save your map in the editor to share it with friends.",
            "Tip: Use the emote wheel to express yourself!",
            "Tip: Vehicles let you travel across the world faster.",
            "Tip: The background layer doesn't block movement.",
        };

        void Awake()
        {
            Instance = this;
            loadingPanel?.SetActive(false);
        }

        public void Show(string message = "Loading...")
        {
            loadingPanel?.SetActive(true);
            if (statusText != null) statusText.text = message;
            if (progressBar != null) progressBar.value = 0f;
            if (tipText    != null) tipText.text = Tips[Random.Range(0, Tips.Length)];
        }

        public void Hide()
        {
            loadingPanel?.SetActive(false);
        }

        public void SetProgress(float t, string message = null)
        {
            if (progressBar != null) progressBar.value = t;
            if (message != null && statusText != null) statusText.text = message;
        }

        public IEnumerator ShowFor(float seconds, string message = "Loading...")
        {
            Show(message);
            float elapsed = 0f;
            while (elapsed < seconds)
            {
                elapsed += Time.deltaTime;
                SetProgress(elapsed / seconds);
                yield return null;
            }
            Hide();
        }
    }
}
