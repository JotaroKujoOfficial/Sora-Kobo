using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;
using SoraKobo.Building;
using SoraKobo.Data;

namespace SoraKobo.MapEditor
{
    public class MapEditorController : MonoBehaviour
    {
        public static MapEditorController Instance { get; private set; }

        [Header("UnityEngine.Camera")]
        public UnityEngine.Camera editorCamera;
        public float panSpeed = 10f;
        public float zoomSpeed = 3f;
        public float minZoom = 2f;
        public float maxZoom = 20f;

        [Header("Grid")]
        public int gridWidth  = 100;
        public int gridHeight = 50;
        public float blockSize = 1f;

        [Header("Palette")]
        public Transform paletteContainer;
        public GameObject paletteBtnPrefab;

        [Header("Current State")]
        public string selectedBlockId = "grass";
        public int selectedLayer = 1;
        public bool eraseMode = false;

        [Header("Save/Load UI")]
        public TMP_InputField mapNameInput;
        public GameObject savePanel;
        public GameObject loadPanel;
        public Transform mapListContainer;
        public GameObject mapListItemPrefab;

        // Local grid (offline editor, no network needed)
        // layer -> pos -> blockId
        private Dictionary<int, Dictionary<Vector2Int, string>> _editorGrid
            = new Dictionary<int, Dictionary<Vector2Int, string>>();
        private Dictionary<int, Dictionary<Vector2Int, GameObject>> _editorObjects
            = new Dictionary<int, Dictionary<Vector2Int, GameObject>>();

        [Header("Block Prefab")]
        public GameObject blockPrefab;

        private bool _isPainting = false;
        private Vector2 _lastPanPos;
        private bool _panning = false;

        void Awake()
        {
            Instance = this;
            for (int i = 0; i < 3; i++)
            {
                _editorGrid[i]   = new Dictionary<Vector2Int, string>();
                _editorObjects[i] = new Dictionary<Vector2Int, GameObject>();
            }
        }

        void Start()
        {
            BuildPalette();
            savePanel?.SetActive(false);
            loadPanel?.SetActive(false);
        }

        void Update()
        {
            HandleEditorInput();
        }

        // ── Input ─────────────────────────────────────────────────────────

        void HandleEditorInput()
        {
            if (Input.touchCount == 0 && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            {
                _isPainting = false;
                _panning = false;
            }

            // Two-finger pan/zoom
            if (Input.touchCount == 2)
            {
                HandlePinch();
                return;
            }

            // Single touch or mouse
            if (Input.touchCount == 1 || Input.GetMouseButton(0))
            {
                Vector2 screenPos = Input.touchCount > 0
                    ? (Vector2)Input.GetTouch(0).position
                    : (Vector2)Input.mousePosition;

                Vector3 worldPos = editorCamera.ScreenToWorldPoint(
                    new Vector3(screenPos.x, screenPos.y, -editorCamera.transform.position.z));
                Vector2Int gridPos = WorldToGrid(worldPos);

                if (eraseMode)
                    RemoveBlock(gridPos, selectedLayer);
                else
                    PlaceBlock(selectedBlockId, gridPos, selectedLayer);
            }

            // Right mouse / two-finger drag = pan (editor only for desktop testing)
            if (Input.GetMouseButton(1))
            {
                Vector2 delta = (Vector2)Input.mousePosition - _lastPanPos;
                editorCamera.transform.Translate(-delta * 0.02f);
            }
            _lastPanPos = Input.mousePosition;

            // Scroll wheel zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.001f)
                AdjustZoom(-scroll * zoomSpeed * 5f);
        }

        void HandlePinch()
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prev0 = t0.position - t0.deltaPosition;
            Vector2 prev1 = t1.position - t1.deltaPosition;

            float prevDist = (prev0 - prev1).magnitude;
            float currDist = (t0.position - t1.position).magnitude;
            float diff = prevDist - currDist;

            AdjustZoom(diff * zoomSpeed * Time.deltaTime);

            // Pan
            Vector2 midpoint     = (t0.position + t1.position) / 2f;
            Vector2 prevMidpoint = (prev0 + prev1) / 2f;
            Vector2 delta = midpoint - prevMidpoint;

            float camZ = editorCamera.orthographicSize / 5f;
            editorCamera.transform.Translate(-delta.x * camZ * Time.deltaTime,
                                             -delta.y * camZ * Time.deltaTime, 0f);
        }

        void AdjustZoom(float amount)
        {
            editorCamera.orthographicSize = Mathf.Clamp(
                editorCamera.orthographicSize + amount, minZoom, maxZoom);
        }

        // ── Block Operations ──────────────────────────────────────────────

        void PlaceBlock(string id, Vector2Int pos, int layer)
        {
            if (pos.x < 0 || pos.x >= gridWidth || pos.y < 0 || pos.y >= gridHeight) return;
            if (_editorGrid[layer].ContainsKey(pos)) return;

            _editorGrid[layer][pos] = id;

            if (blockPrefab != null)
            {
                var go = Instantiate(blockPrefab, GridToWorld(pos), Quaternion.identity);
                var sr = go.GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = PlacedBlock.GetColor(id);
                // Remove network components for editor
                Destroy(go.GetComponent<Mirror.NetworkIdentity>());
                Destroy(go.GetComponent<PlacedBlock>());
                _editorObjects[layer][pos] = go;
            }
        }

        void RemoveBlock(Vector2Int pos, int layer)
        {
            if (!_editorGrid[layer].ContainsKey(pos)) return;
            _editorGrid[layer].Remove(pos);
            if (_editorObjects[layer].TryGetValue(pos, out var go))
            {
                Destroy(go);
                _editorObjects[layer].Remove(pos);
            }
        }

        public void ClearAll()
        {
            for (int i = 0; i < 3; i++)
            {
                foreach (var go in _editorObjects[i].Values) Destroy(go);
                _editorObjects[i].Clear();
                _editorGrid[i].Clear();
            }
        }

        // ── Palette ───────────────────────────────────────────────────────

        void BuildPalette()
        {
            if (paletteContainer == null || paletteBtnPrefab == null) return;
            var blocks = PlacedBlock.GetAllBlockIds();
            foreach (var id in blocks)
            {
                var go  = Instantiate(paletteBtnPrefab, paletteContainer);
                var btn = go.GetComponent<Button>();
                var img = go.GetComponent<Image>();
                var txt = go.GetComponentInChildren<TextMeshProUGUI>();

                if (img != null) img.color = PlacedBlock.GetColor(id);
                if (txt != null) txt.text  = char.ToUpper(id[0]) + id.Substring(1);

                string captured = id;
                btn?.onClick.AddListener(() =>
                {
                    selectedBlockId = captured;
                    eraseMode = false;
                });
            }
        }

        public void SelectBlock(string id) { selectedBlockId = id; eraseMode = false; }
        public void SetEraseMode(bool e)   { eraseMode = e; }
        public void SelectLayer(int l)     { selectedLayer = l; }

        // ── Save / Load ───────────────────────────────────────────────────

        public void OnSaveButton()
        {
            savePanel?.SetActive(true);
        }

        public void OnConfirmSave()
        {
            string name = mapNameInput != null ? mapNameInput.text.Trim() : "MyMap";
            if (string.IsNullOrEmpty(name)) name = "MyMap";
            SaveMap(name);
            savePanel?.SetActive(false);
        }

        public void OnLoadButton()
        {
            RefreshMapList();
            loadPanel?.SetActive(true);
        }

        public void OnCloseSave() => savePanel?.SetActive(false);
        public void OnCloseLoad() => loadPanel?.SetActive(false);

        void SaveMap(string mapName)
        {
            var data = new MapSaveData
            {
                mapName  = mapName,
                authorName = PlayerPrefs.GetString("PlayerName", "Player"),
                width    = gridWidth,
                height   = gridHeight,
                saveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            };

            for (int l = 0; l < 3; l++)
                foreach (var kvp in _editorGrid[l])
                    data.blocks.Add(new PlacedBlockData { blockId = kvp.Value, x = kvp.Key.x, y = kvp.Key.y, layer = l });

            string json = JsonUtility.ToJson(data, true);
            string path = GetSavePath(mapName);
            File.WriteAllText(path, json);

            // Also save as CurrentMap for multiplayer
            PlayerPrefs.SetString("CurrentMap", json);
            PlayerPrefs.Save();

            Debug.Log($"[SoraKobo] Map saved: {path}");
        }

        public void LoadMap(string mapName)
        {
            string path = GetSavePath(mapName);
            if (!File.Exists(path)) { Debug.LogWarning($"Map not found: {path}"); return; }

            string json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<MapSaveData>(json);

            ClearAll();
            foreach (var b in data.blocks)
                PlaceBlock(b.blockId, new Vector2Int(b.x, b.y), b.layer);

            PlayerPrefs.SetString("CurrentMap", json);
            PlayerPrefs.Save();

            loadPanel?.SetActive(false);
        }

        void RefreshMapList()
        {
            if (mapListContainer == null) return;
            foreach (Transform t in mapListContainer) Destroy(t.gameObject);

            string dir = GetSaveDirectory();
            if (!Directory.Exists(dir)) return;

            foreach (var file in Directory.GetFiles(dir, "*.json"))
            {
                string mapName = Path.GetFileNameWithoutExtension(file);
                if (mapListItemPrefab == null) continue;
                var go = Instantiate(mapListItemPrefab, mapListContainer);
                var txt = go.GetComponentInChildren<TextMeshProUGUI>();
                var btn = go.GetComponent<Button>();
                if (txt != null) txt.text = mapName;
                string captured = mapName;
                btn?.onClick.AddListener(() => LoadMap(captured));
            }
        }

        string GetSaveDirectory()
        {
            string dir = Path.Combine(Application.persistentDataPath, "Maps");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return dir;
        }

        string GetSavePath(string mapName) =>
            Path.Combine(GetSaveDirectory(), mapName + ".json");

        // ── Coordinate Helpers ────────────────────────────────────────────

        Vector2Int WorldToGrid(Vector3 world) => new Vector2Int(
            Mathf.RoundToInt(world.x / blockSize),
            Mathf.RoundToInt(world.y / blockSize));

        Vector3 GridToWorld(Vector2Int grid) =>
            new Vector3(grid.x * blockSize, grid.y * blockSize, 0f);

        // ── Public API for UndoRedo ───────────────────────────────────────
        public void PlaceBlock_Editor(string id, Vector2Int pos, int layer) => PlaceBlock(id, pos, layer);
        public void RemoveBlock_Editor(Vector2Int pos, int layer)           => RemoveBlock(pos, layer);

        // ── Navigation ────────────────────────────────────────────────────

        public void OnBackToMenu()
        {
            UI.UIManager.Instance?.ShowScreen("MainMenu");
        }

        public void OnTestInMultiplayer()
        {
            // Save current map and go to menu to host
            string mapName = "EditorMap_" + System.DateTime.Now.Ticks;
            SaveMap(mapName);
            UI.UIManager.Instance?.ShowScreen("MainMenu");
        }
    }
}
