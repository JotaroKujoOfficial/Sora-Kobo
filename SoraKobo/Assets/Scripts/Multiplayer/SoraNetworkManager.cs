using UnityEngine;
using Mirror;
using System.Collections.Generic;

namespace SoraKobo.Multiplayer
{
    [System.Serializable]
    public class ServerInfo
    {
        public string serverName;
        public string ip;
        public int port;
        public int playerCount;
        public int maxPlayers;
    }

    public class SoraNetworkManager : NetworkManager
    {
        public static new SoraNetworkManager Instance { get; private set; }

        [Header("Sora Settings")]
        public string serverName      = "Sora Kobo Server";
        public int maxPlayersPerServer = 10;

        [Header("Spawn Points")]
        public Transform[] spawnPoints;

        // ── Static events — UI subscribes, no direct reference needed ─────
        public static event System.Action<string> OnScreenRequested;   // screen name
        public static event System.Action<int>    OnPlayerCountChanged;

        public readonly List<SoraPlayerData> connectedPlayers = new List<SoraPlayerData>();
        private bool _worldInitialized;
        private int  _spawnIndex;

        // ── Lifecycle ─────────────────────────────────────────────────────

        public override void Awake()
        {
            base.Awake();
            Instance       = this;
            maxConnections = maxPlayersPerServer;
        }

        // ── Server Callbacks ──────────────────────────────────────────────

        public override void OnStartServer()
        {
            base.OnStartServer();
            _worldInitialized = false;
            _spawnIndex       = 0;
            Debug.Log("[SoraKobo] Server started.");
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
            Debug.Log($"[SoraKobo] Client connected: {conn.connectionId}");
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            // Release resources before base destroys the player object
            var handler = FindObjectOfType<DisconnectHandler>();
            handler?.HandlePlayerDisconnect(conn);

            base.OnServerDisconnect(conn);
            connectedPlayers.RemoveAll(p => p.connectionId == conn.connectionId);
            Debug.Log($"[SoraKobo] Client disconnected: {conn.connectionId}");
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Transform start  = GetSpawnPoint();
            var player       = Instantiate(playerPrefab, start.position, start.rotation);
            NetworkServer.AddPlayerForConnection(conn, player);

            connectedPlayers.Add(new SoraPlayerData
            {
                connectionId = conn.connectionId,
                playerName   = "Player_" + conn.connectionId
            });

            if (!_worldInitialized)
            {
                _worldInitialized = true;
                InitializeWorld();
            }
            else
            {
                SendCurrentMapToClient(conn);
            }
        }

        // ── Client Callbacks ──────────────────────────────────────────────

        public override void OnStartClient()
        {
            base.OnStartClient();
            // Register client-side message handlers once per session
            NetworkMessages.Handlers.RegisterClient();
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            Debug.Log("[SoraKobo] Connected to server!");
            OnScreenRequested?.Invoke("Game");
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            Debug.Log("[SoraKobo] Disconnected from server.");
            OnScreenRequested?.Invoke("MainMenu");
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            OnScreenRequested?.Invoke("MainMenu");
        }

        // ── Host / Join ───────────────────────────────────────────────────

        public void HostGame(string ip, int port, string servName)
        {
            networkAddress = ip;
            SetPort((ushort)port);
            serverName = servName;
            StartHost();
        }

        public void JoinGame(string ip, int port)
        {
            networkAddress = ip;
            SetPort((ushort)port);
            StartClient();
        }

        public void LeaveGame()
        {
            if (NetworkServer.active && NetworkClient.isConnected) StopHost();
            else if (NetworkClient.isConnected)                    StopClient();
            else if (NetworkServer.active)                         StopServer();
        }

        // ── Helpers ───────────────────────────────────────────────────────

        void SetPort(ushort port)
        {
            if (Transport.active is kcp2k.KcpTransport kcp) kcp.Port = port;
        }

        Transform GetSpawnPoint()
        {
            if (spawnPoints == null || spawnPoints.Length == 0) return transform;
            return spawnPoints[_spawnIndex++ % spawnPoints.Length];
        }

        [Server]
        void InitializeWorld()
        {
            var building = FindObjectOfType<Building.BuildingSystem>();
            if (building == null) return;
            string saved = PlayerPrefs.GetString("CurrentMap", "");
            if (!string.IsNullOrEmpty(saved)) building.ServerLoadMap(saved);
            else                               building.GenerateDefaultMap();
        }

        [Server]
        void SendCurrentMapToClient(NetworkConnectionToClient conn)
        {
            string json = PlayerPrefs.GetString("CurrentMap", "");
            if (!string.IsNullOrEmpty(json))
                conn.Send(new NetworkMessages.MapDataMessage { json = json });
        }
    }

    [System.Serializable]
    public class SoraPlayerData
    {
        public int    connectionId;
        public string playerName;
    }
}
