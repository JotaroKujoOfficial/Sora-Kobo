using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace SoraKobo.Building
{
    /// <summary>
    /// Scrollable block palette for the in-game build mode.
    /// Generates one button per block type automatically.
    /// </summary>
    public class BlockPalette : MonoBehaviour
    {
        [Header("UI")]
        public Transform buttonContainer;
        public GameObject buttonPrefab;
        public ScrollRect scrollRect;

        [Header("Selection Highlight")]
        public Color selectedColor  = new Color(1f, 0.9f, 0.3f);
        public Color normalColor    = Color.white;

        private string _selectedId;
        private List<(string id, Button btn)> _buttons = new List<(string, Button)>();

        void Start()
        {
            GenerateButtons();
        }

        void GenerateButtons()
        {
            if (buttonContainer == null || buttonPrefab == null) return;

            var ids = PlacedBlock.GetAllBlockIds();
            foreach (var id in ids)
            {
                var go  = Instantiate(buttonPrefab, buttonContainer);
                var btn = go.GetComponent<Button>();
                var img = go.GetComponent<Image>();
                var txt = go.GetComponentInChildren<TextMeshProUGUI>();

                if (img != null) img.color = PlacedBlock.GetColor(id);
                if (txt != null) txt.text  = Capitalize(id);

                string captured = id;
                btn?.onClick.AddListener(() => Select(captured));

                _buttons.Add((id, btn));
            }
        }

        void Select(string id)
        {
            _selectedId = id;
            BuildingSystem.Instance?.SelectBlock(id);
            BuildingSystem.Instance?.SetEraseMode(false);

            foreach (var (bid, btn) in _buttons)
            {
                var img = btn?.GetComponent<Image>();
                if (img != null)
                    img.color = bid == id ? selectedColor : PlacedBlock.GetColor(bid);
            }
        }

        static string Capitalize(string s)
            => s.Length > 0 ? char.ToUpper(s[0]) + s.Substring(1) : s;
    }
}
