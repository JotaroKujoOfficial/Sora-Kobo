using UnityEngine;

namespace SoraKobo.UI
{
    /// <summary>
    /// Simple floating particle effect for the main menu.
    /// Creates small colourful sprites that float upward — cozy ambiance.
    /// Attach to an empty GameObject in the UI scene.
    /// </summary>
    public class StarfieldEffect : MonoBehaviour
    {
        [Header("Particles")]
        public int count     = 30;
        public float speed   = 60f;
        public float minSize = 8f;
        public float maxSize = 20f;
        public Color[] colors =
        {
            new Color(1f, 0.9f, 0.4f, 0.8f),
            new Color(0.4f, 0.9f, 1f, 0.8f),
            new Color(1f, 0.5f, 0.8f, 0.8f),
            new Color(0.6f, 1f, 0.6f, 0.8f),
        };

        private struct Particle
        {
            public GameObject go;
            public float speedMult;
        }

        private Particle[] _particles;
        private RectTransform _rect;
        private Canvas _canvas;

        void Start()
        {
            _rect   = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            _particles = new Particle[count];
            for (int i = 0; i < count; i++)
                _particles[i] = CreateParticle(true);
        }

        void Update()
        {
            for (int i = 0; i < _particles.Length; i++)
            {
                var p = _particles[i];
                if (p.go == null) continue;
                p.go.transform.localPosition += Vector3.up * speed * p.speedMult * Time.deltaTime;

                // Wrap around
                if (p.go.transform.localPosition.y > Screen.height * 0.6f)
                {
                    Destroy(p.go);
                    _particles[i] = CreateParticle(false);
                }
            }
        }

        Particle CreateParticle(bool randomY)
        {
            var go = new GameObject("Star");
            go.transform.SetParent(transform, false);

            var rt = go.AddComponent<RectTransform>();
            float size = Random.Range(minSize, maxSize);
            rt.sizeDelta = new Vector2(size, size);

            float x = Random.Range(-Screen.width * 0.5f, Screen.width * 0.5f);
            float y = randomY
                ? Random.Range(-Screen.height * 0.5f, Screen.height * 0.5f)
                : -Screen.height * 0.5f - size;
            rt.anchoredPosition = new Vector2(x, y);

            var img = go.AddComponent<UnityEngine.UI.Image>();
            img.color = colors[Random.Range(0, colors.Length)];

            return new Particle { go = go, speedMult = Random.Range(0.5f, 1.5f) };
        }
    }
}
