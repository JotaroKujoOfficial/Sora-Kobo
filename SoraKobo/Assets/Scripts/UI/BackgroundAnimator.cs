using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SoraKobo.UI
{
    /// <summary>
    /// Animates the main menu background with a soft color cycle.
    /// Attach to the background Image of the main menu canvas.
    /// </summary>
    public class BackgroundAnimator : MonoBehaviour
    {
        [Header("Colors")]
        public Color[] colors =
        {
            new Color(0.53f, 0.81f, 0.98f),   // sky blue
            new Color(0.98f, 0.85f, 0.53f),   // warm peach
            new Color(0.72f, 0.95f, 0.72f),   // soft green
            new Color(0.95f, 0.72f, 0.87f),   // soft pink
        };

        [Header("Settings")]
        public float transitionDuration = 4f;

        private Image _image;
        private int _colorIndex = 0;

        void Start()
        {
            _image = GetComponent<Image>();
            if (_image != null && colors.Length > 0)
                _image.color = colors[0];
            StartCoroutine(CycleColors());
        }

        IEnumerator CycleColors()
        {
            while (true)
            {
                int next = (_colorIndex + 1) % colors.Length;
                Color from = colors[_colorIndex];
                Color to   = colors[next];
                float t    = 0f;

                while (t < transitionDuration)
                {
                    t += Time.deltaTime;
                    if (_image != null)
                        _image.color = Color.Lerp(from, to, t / transitionDuration);
                    yield return null;
                }

                _colorIndex = next;
            }
        }
    }
}
