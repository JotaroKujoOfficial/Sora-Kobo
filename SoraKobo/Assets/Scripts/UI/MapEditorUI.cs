using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SoraKobo.UI
{
    public class MapEditorUI : MonoBehaviour
    {
        [Header("Toolbar")]
        public Button buildBtn;
        public Button eraseBtn;
        public Button saveBtn;
        public Button loadBtn;
        public Button clearBtn;
        public Button backBtn;

        [Header("Layer Selector")]
        public Button[] layerButtons; // 0=BG, 1=Main, 2=FG
        public Color selectedLayerColor = new Color(0.3f, 0.8f, 0.3f);
        public Color normalLayerColor   = Color.white;

        [Header("Info")]
        public TextMeshProUGUI modeText;
        public TextMeshProUGUI coordText;

        private MapEditor.MapEditorController _editor;
        private bool _eraseMode = false;
        private int _selectedLayer = 1;
        private Camera _cam;

        void Start()
        {
            _editor = MapEditor.MapEditorController.Instance;
            _cam    = UnityEngine.UnityEngine.Camera.main;

            buildBtn?.onClick.AddListener(OnBuild);
            eraseBtn?.onClick.AddListener(OnErase);
            saveBtn?.onClick.AddListener(OnSave);
            loadBtn?.onClick.AddListener(OnLoad);
            clearBtn?.onClick.AddListener(OnClear);
            backBtn?.onClick.AddListener(OnBack);

            for (int i = 0; i < layerButtons?.Length; i++)
            {
                int captured = i;
                layerButtons[i]?.onClick.AddListener(() => SelectLayer(captured));
            }

            SelectLayer(1);
            OnBuild();
        }

        void Update()
        {
            UpdateCoordDisplay();
        }

        void OnBuild()
        {
            _eraseMode = false;
            _editor?.SetEraseMode(false);
            if (modeText != null) modeText.text = "Mode: Build";
            HighlightButton(buildBtn, true);
            HighlightButton(eraseBtn, false);
        }

        void OnErase()
        {
            _eraseMode = true;
            _editor?.SetEraseMode(true);
            if (modeText != null) modeText.text = "Mode: Erase";
            HighlightButton(eraseBtn, true);
            HighlightButton(buildBtn, false);
        }

        void OnSave()  => _editor?.OnSaveButton();
        void OnLoad()  => _editor?.OnLoadButton();
        void OnClear() => _editor?.ClearAll();
        void OnBack()  => _editor?.OnBackToMenu();

        void SelectLayer(int layer)
        {
            _selectedLayer = layer;
            _editor?.SelectLayer(layer);
            for (int i = 0; i < layerButtons?.Length; i++)
            {
                var img = layerButtons[i]?.GetComponent<Image>();
                if (img != null) img.color = i == layer ? selectedLayerColor : normalLayerColor;
            }
        }

        void HighlightButton(Button btn, bool on)
        {
            var img = btn?.GetComponent<Image>();
            if (img != null) img.color = on ? new Color(0.4f, 0.85f, 0.4f) : Color.white;
        }

        void UpdateCoordDisplay()
        {
            if (coordText == null || _cam == null) return;
            Vector3 world = _cam.ScreenToWorldPoint(
                new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, -_cam.transform.position.z));
            coordText.text = $"({Mathf.RoundToInt(world.x)}, {Mathf.RoundToInt(world.y)})";
        }
    }
}
