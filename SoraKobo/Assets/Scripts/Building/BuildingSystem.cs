using UnityEngine;
using Mirror;
using System.Collections.Generic;
using SoraKobo.Data;

namespace SoraKobo.Building
{
    public class BuildingSystem : NetworkBehaviour
    {
        public static BuildingSystem Instance { get; private set; }

        [Header("Block Settings")]
        public GameObject blockPrefab;
        public float blockSize  = 1f;
        public int   gridWidth  = 100;
        public int   gridHeight = 50;

        [Header("Max Distance")]
        public float maxBuildDistance = 12f; // server-side anti-cheat radius

        // Grid: layer → position → netId
        private readonly Dictionary<int, Dictionary<Vector2Int, uint>> _grid
            = new Dictionary<int, Dictionary<Vector2Int, uint>>();

        // Client-side ghost preview (no NetworkIdentity)
        private GameObject _ghostBlock;
        private bool _buildMode;
        private string _selectedBlockId = "grass";
        private int  _selectedLayer    = 1;
        private bool _eraseMode;
        private Camera _cam;

        // ── Lifecycle ─────────────────────────────────────────────────────

        void Awake()
        {
            Instance = this;
            for (int i = 0; i < 3; i++)
                _grid[i] = new Dictionary<Vector2Int, uint>();
        }

        void Start()
        {
            _cam = Camera.main;
            CreateGhost();
        }

        void CreateGhost()
        {
            if (blockPrefab == null) return;
            _ghostBlock = Instantiate(blockPrefab);
            _ghostBlock.SetActive(false);
            Destroy(_ghostBlock.GetComponent<NetworkIdentity>());
            Destroy(_ghostBlock.GetComponent<PlacedBlock>());
            var col = _ghostBlock.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
            var sr = _ghostBlock.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = new Color(1f, 1f, 1f, 0.5f);
        }

        void Update()
        {
            // Ghost is only meaningful for the local player.
            // BuildingSystem is a scene object, so we check _buildMode flag
            // that only the local player sets via SetBuildMode().
            if (!isClient || !_buildMode) { _ghostBlock?.SetActive(false); return; }
            UpdateGhost();
        }

        void UpdateGhost()
        {
            if (_cam == null) return;
            Vector2Int gridPos = WorldToGrid(GetPointerWorldPosition());
            if (_ghostBlock != null)
            {
                _ghostBlock.SetActive(true);
                _ghostBlock.transform.position = GridToWorld(gridPos);
                var sr = _ghostBlock.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.color = _eraseMode
                        ? new Color(1f, 0.3f, 0.3f, 0.5f)
                        : new Color(1f, 1f, 1f, 0.5f);
            }
        }

        // ── Touch/Tap API (called by BuildTouchInput) ─────────────────────

        public void OnTapBuild(Vector2 screenPos)
        {
            if (!_buildMode || _cam == null) return;
            Vector3 world = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -_cam.transform.position.z));
            world.z = 0;
            Vector2Int gridPos = WorldToGrid(world);
            if (_eraseMode)
                CmdRemoveBlock(gridPos, _selectedLayer);
            else
                CmdPlaceBlock(_selectedBlockId, gridPos, _selectedLayer);
        }

        // ── Commands (server authority) ───────────────────────────────────

        [Command]
        public void CmdPlaceBlock(string blockId, Vector2Int pos, int layer,
            NetworkConnectionToClient sender = null)
        {
            // Server-side validation
            if (!IsValidGridPos(pos)) return;
            if (_grid[layer].ContainsKey(pos)) return;
            if (!IsValidBlockId(blockId)) return;

            // Proximity check: the sender's player must be within build range
            if (sender != null && sender.identity != null)
            {
                Vector3 playerPos = sender.identity.transform.position;
                Vector3 blockWorld = GridToWorld(pos);
                if (Vector2.Distance(playerPos, blockWorld) > maxBuildDistance) return;
            }

            ServerPlaceBlock(blockId, pos, layer);
        }

        [Command]
        public void CmdRemoveBlock(Vector2Int pos, int layer,
            NetworkConnectionToClient sender = null)
        {
            if (!_grid[layer].ContainsKey(pos)) return;

            // Proximity check
            if (sender != null && sender.identity != null)
            {
                Vector3 playerPos = sender.identity.transform.position;
                if (Vector2.Distance(playerPos, GridToWorld(pos)) > maxBuildDistance) return;
            }

            ServerRemoveBlock(pos, layer);
        }

        // ── Server-only internal block operations ─────────────────────────

        [Server]
        private void ServerPlaceBlock(string blockId, Vector2Int pos, int layer)
        {
            var go = Instantiate(blockPrefab, GridToWorld(pos), Quaternion.identity);
            var blockComp = go.GetComponent<PlacedBlock>();
            if (blockComp != null) blockComp.blockId = blockId;
            NetworkServer.Spawn(go);
            _grid[layer][pos] = go.GetComponent<NetworkIdentity>().netId;
        }

        [Server]
        private void ServerRemoveBlock(Vector2Int pos, int layer)
        {
            uint netId = _grid[layer][pos];
            _grid[layer].Remove(pos);
            if (NetworkServer.spawned.TryGetValue(netId, out var identity))
                NetworkServer.Destroy(identity.gameObject);
        }

        // ── Map load/save ─────────────────────────────────────────────────

        /// <summary>Called on clients when they receive the map from the server.</summary>
        public void ClientReceiveMap(string json)
        {
            // Replaying the block list client-side: clear local ghost grid only
            // (server has authority; we just visualise what the server already spawned)
            Debug.Log("[SoraKobo] Client received map data, length=" + json.Length);
            // Nothing extra needed: server spawns blocks via NetworkServer.Spawn
            // which auto-propagates to all clients. This method is a hook for
            // future client-only visual effects (fade-in, etc.)
        }

        [Server]
        public void ServerLoadMap(string json)
        {
            // Clear existing blocks
            for (int l = 0; l < 3; l++)
            {
                var positions = new List<Vector2Int>(_grid[l].Keys);
                foreach (var pos in positions)
                    ServerRemoveBlock(pos, l);
            }

            var data = JsonUtility.FromJson<MapSaveData>(json);
            if (data == null) return;
            foreach (var b in data.blocks)
                ServerPlaceBlock(b.blockId, new Vector2Int(b.x, b.y), b.layer);
        }

        [Server]
        public void GenerateDefaultMap()
        {
            var gen = GetComponent<WorldGenerator>() ?? FindObjectOfType<WorldGenerator>();
            if (gen != null) { gen.Generate(); return; }

            // Fallback minimal terrain
            int groundY = 5;
            for (int x = 0; x < gridWidth; x++)
            {
                ServerPlaceBlock("grass", new Vector2Int(x, groundY), 1);
                for (int y = 0; y < groundY; y++)
                    ServerPlaceBlock("dirt", new Vector2Int(x, y), 1);
                for (int y = groundY + 1; y < gridHeight; y++)
                    ServerPlaceBlock("sky", new Vector2Int(x, y), 0);
            }
        }

        // ── Mode controls (called by HUD/local player only) ───────────────

        public void SetBuildMode(bool active, bool erase = false)
        {
            _buildMode  = active;
            _eraseMode  = erase;
            if (!active && _ghostBlock != null) _ghostBlock.SetActive(false);
        }

        public void SelectBlock(string blockId)  => _selectedBlockId = blockId;
        public void SelectLayer(int layer)        => _selectedLayer   = layer;
        public void SetEraseMode(bool erase)      => _eraseMode       = erase;

        // ── Helpers ───────────────────────────────────────────────────────

        Vector3 GetPointerWorldPosition()
        {
            if (_cam == null) return Vector3.zero;
            Vector3 sp = Input.touchCount > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition;
            sp.z = -_cam.transform.position.z;
            return _cam.ScreenToWorldPoint(sp);
        }

        public Vector2Int WorldToGrid(Vector3 world) => new Vector2Int(
            Mathf.RoundToInt(world.x / blockSize),
            Mathf.RoundToInt(world.y / blockSize));

        public Vector3 GridToWorld(Vector2Int grid) =>
            new Vector3(grid.x * blockSize, grid.y * blockSize, 0f);

        bool IsValidGridPos(Vector2Int pos) =>
            pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;

        bool IsValidBlockId(string id) =>
            !string.IsNullOrEmpty(id) && PlacedBlock.IsKnownBlock(id);
    }
}
