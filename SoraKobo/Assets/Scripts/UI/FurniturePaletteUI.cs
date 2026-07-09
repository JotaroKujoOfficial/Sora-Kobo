using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SoraKobo.UI
{
    public class FurniturePaletteUI : MonoBehaviour
    {
        [Header("UI")]
        public Transform buttonContainer;
        public GameObject buttonPrefab;

        [Header("Items")]
        public string[] furnitureNames = { "Chair", "Piano", "Door", "Swing", "Food", "Sign" };
        public Color[]  furnitureColors =
        {
            new Color(0.72f, 0.52f, 0.25f),  // Chair - wood
            new Color(0.20f, 0.20f, 0.20f),  // Piano - black
            new Color(0.55f, 0.35f, 0.15f),  // Door  - brown
            new Color(0.53f, 0.80f, 0.92f),  // Swing - blue
            new Color(0.95f, 0.30f, 0.30f),  // Food  - red
            new Color(0.93f, 0.87f, 0.60f),  // Sign  - sand
        };

        private Building.FurniturePlacer _placer;

        void Start()
        {
            _placer = FindObjectOfType<Building.FurniturePlacer>();
            BuildPalette();
        }

        void BuildPalette()
        {
            if (buttonContainer == null || buttonPrefab == null) return;

            for (int i = 0; i < furnitureNames.Length; i++)
            {
                var go  = Instantiate(buttonPrefab, buttonContainer);
                var btn = go.GetComponent<Button>();
                var img = go.GetComponent<Image>();
                var txt = go.GetComponentInChildren<TextMeshProUGUI>();

                if (i < furnitureColors.Length && img != null)
                    img.color = furnitureColors[i];
                if (txt != null) txt.text = furnitureNames[i];

                int captured = i;
                btn?.onClick.AddListener(() => _placer?.SelectFurniture(captured));
            }
        }
    }
}
