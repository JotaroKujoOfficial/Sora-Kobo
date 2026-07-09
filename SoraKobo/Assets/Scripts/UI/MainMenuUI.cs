using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SoraKobo.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Main Menu Panels")]
        public GameObject mainPanel;
        public GameObject joinPanel;
        public GameObject hostPanel;
        public GameObject settingsPanel;

        [Header("Join Panel")]
        public TMP_InputField joinIpInput;
        public TMP_InputField joinPortInput;
        public Button joinButton;
        public TextMeshProUGUI joinStatusText;

        [Header("Host Panel")]
        public TMP_InputField hostPortInput;
        public TMP_InputField serverNameInput;
        public Button hostButton;

        [Header("Settings")]
        public TMP_InputField playerNameInput;
        public Button saveSettingsButton;

        [Header("Version")]
        public TextMeshProUGUI versionText;

        void Start()
        {
            if (versionText != null)
                versionText.text = "Sora Kobo v1.0";

            // Load saved name
            if (playerNameInput != null)
                playerNameInput.text = PlayerPrefs.GetString("PlayerName", "Player");

            ShowPanel(mainPanel);
        }

        // ── Navigation ────────────────────────────────────────────────────

        public void OnPlayButton()       => ShowPanel(joinPanel);
        public void OnHostButton()       => ShowPanel(hostPanel);
        public void OnSettingsButton()   => ShowPanel(settingsPanel);
        public void OnBackButton()       => ShowPanel(mainPanel);
        public void OnCustomizeButton()  => UIManager.Instance?.ShowScreen("Customization");
        public void OnMapEditorButton()  => UIManager.Instance?.ShowScreen("MapEditor");

        public void OnQuitButton()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // ── Join ──────────────────────────────────────────────────────────

        public void OnJoinConfirm()
        {
            string ip   = joinIpInput   != null ? joinIpInput.text.Trim()   : "127.0.0.1";
            string portStr = joinPortInput != null ? joinPortInput.text.Trim() : "7777";

            if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";
            if (!int.TryParse(portStr, out int port)) port = 7777;

            if (joinStatusText != null) joinStatusText.text = "Connecting...";
            if (joinButton     != null) joinButton.interactable = false;

            Multiplayer.SoraNetworkManager.Instance?.JoinGame(ip, port);
        }

        // ── Host ──────────────────────────────────────────────────────────

        public void OnHostConfirm()
        {
            string portStr = hostPortInput  != null ? hostPortInput.text.Trim()  : "7777";
            string name    = serverNameInput != null ? serverNameInput.text.Trim() : "My Server";

            if (!int.TryParse(portStr, out int port)) port = 7777;
            if (string.IsNullOrEmpty(name)) name = "Sora Kobo Server";

            Multiplayer.SoraNetworkManager.Instance?.HostGame("0.0.0.0", port, name);
        }

        // ── Settings ──────────────────────────────────────────────────────

        public void OnSaveSettings()
        {
            if (playerNameInput != null)
            {
                string n = playerNameInput.text.Trim();
                if (string.IsNullOrEmpty(n)) n = "Player";
                PlayerPrefs.SetString("PlayerName", n);
                PlayerPrefs.Save();
            }
            ShowPanel(mainPanel);
        }

        // ── Helpers ───────────────────────────────────────────────────────

        void ShowPanel(GameObject panel)
        {
            mainPanel?.SetActive(panel == mainPanel);
            joinPanel?.SetActive(panel == joinPanel);
            hostPanel?.SetActive(panel == hostPanel);
            settingsPanel?.SetActive(panel == settingsPanel);

            // Reset status
            if (joinStatusText != null) joinStatusText.text = "";
            if (joinButton     != null) joinButton.interactable = true;
        }
    }
}
