using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace SoraKobo.Multiplayer
{
    /// <summary>
    /// Simple local-network server browser using UDP broadcast discovery.
    /// Falls back to direct IP entry when broadcast is unavailable.
    /// </summary>
    public class ServerBrowser : MonoBehaviour
    {
        [Header("UI")]
        public Transform serverListContainer;
        public GameObject serverEntryPrefab;
        public Button refreshButton;
        public Button directConnectButton;
        public TMP_InputField directIPInput;
        public TMP_InputField directPortInput;
        public TextMeshProUGUI statusText;

        [Header("Discovery")]
        public float discoveryTimeout = 3f;

        private List<ServerInfo> _discovered = new List<ServerInfo>();

        void Start()
        {
            refreshButton?.onClick.AddListener(Refresh);
            directConnectButton?.onClick.AddListener(OnDirectConnect);
        }

        void OnEnable() => Refresh();

        public void Refresh()
        {
            _discovered.Clear();
            ClearList();
            if (statusText != null) statusText.text = "Searching...";

            // In a production game, use LAN discovery via UDP multicast.
            // Here we provide direct-IP fallback and show usage tip.
            if (statusText != null)
                statusText.text = "Enter IP address directly, or host a game to see it here.";
        }

        public void AddServer(ServerInfo info)
        {
            _discovered.Add(info);
            RefreshList();
        }

        void RefreshList()
        {
            ClearList();
            foreach (var info in _discovered)
            {
                if (serverEntryPrefab == null) continue;
                var go = Instantiate(serverEntryPrefab, serverListContainer);

                var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
                if (texts.Length > 0) texts[0].text = info.serverName;
                if (texts.Length > 1) texts[1].text = $"{info.playerCount}/{info.maxPlayers}";
                if (texts.Length > 2) texts[2].text = $"{info.ip}:{info.port}";

                var btn = go.GetComponent<Button>();
                var captured = info;
                btn?.onClick.AddListener(() => ConnectTo(captured));
            }
        }

        void ClearList()
        {
            if (serverListContainer == null) return;
            foreach (Transform t in serverListContainer) Destroy(t.gameObject);
        }

        void ConnectTo(ServerInfo info)
        {
            SoraNetworkManager.Instance?.JoinGame(info.ip, info.port);
        }

        void OnDirectConnect()
        {
            string ip   = directIPInput  != null ? directIPInput.text.Trim()  : "127.0.0.1";
            string port = directPortInput != null ? directPortInput.text.Trim() : "7777";
            if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";
            if (!int.TryParse(port, out int p)) p = 7777;
            SoraNetworkManager.Instance?.JoinGame(ip, p);
        }
    }
}
