using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SoraKobo.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("Build Mode")]
        public GameObject buildPanel;
        public Button buildToggleButton;
        public Button eraseToggleButton;
        public Transform blockButtonContainer;
        public GameObject blockButtonPrefab;

        [Header("Interaction")]
        public GameObject interactPromptPanel;
        public TextMeshProUGUI interactPromptText;
        public Button interactButton;

        [Header("Player HUD")]
        public Button jumpButton;
        public TextMeshProUGUI playerCountText;

        [Header("Sign Popup")]
        public GameObject signPopup;
        public TextMeshProUGUI signContentText;

        [Header("Top Bar")]
        public TextMeshProUGUI serverNameText;
        public Button leaveButton;

        private Player.PlayerController _localPlayer;
        private Building.BuildingSystem _buildingSystem;
        private bool _buildModeActive = false;

        void Start()
        {
            _buildingSystem = Building.BuildingSystem.Instance;
            BuildBlockPalette();
            buildPanel?.SetActive(false);
            interactPromptPanel?.SetActive(false);
            signPopup?.SetActive(false);

            // Subscribe to proximity trigger events (no direct HUD reference in Interaction)
            Interaction.InteractableObject.OnPromptChanged += ShowInteractPrompt;
        }

        void OnDestroy()
        {
            Interaction.InteractableObject.OnPromptChanged -= ShowInteractPrompt;
        }

        public void OnLocalPlayerSpawned(Player.PlayerController player)
        {
            _localPlayer = player;
        }

        // ── Build Mode ────────────────────────────────────────────────────

        public void OnBuildToggle()
        {
            _buildModeActive = !_buildModeActive;
            buildPanel?.SetActive(_buildModeActive);
            _buildingSystem?.SetBuildMode(_buildModeActive, false);
            _localPlayer?.SetBuildMode(_buildModeActive);

            if (buildToggleButton != null)
            {
                var c = buildToggleButton.colors;
                c.normalColor = _buildModeActive ? new Color(0.4f, 0.8f, 0.4f) : Color.white;
                buildToggleButton.colors = c;
            }
        }

        public void OnEraseModeToggle()
        {
            if (!_buildModeActive) return;
            _buildingSystem?.SetEraseMode(true);
        }

        void BuildBlockPalette()
        {
            if (blockButtonContainer == null || blockButtonPrefab == null) return;
            foreach (var id in Building.PlacedBlock.GetAllBlockIds())
            {
                var go  = Instantiate(blockButtonPrefab, blockButtonContainer);
                var btn = go.GetComponent<Button>();
                var img = go.GetComponent<Image>();
                var txt = go.GetComponentInChildren<TextMeshProUGUI>();
                if (img != null) img.color = Building.PlacedBlock.GetColor(id);
                if (txt != null) txt.text  = CapFirst(id);
                string captured = id;
                btn?.onClick.AddListener(() =>
                {
                    _buildingSystem?.SelectBlock(captured);
                    _buildingSystem?.SetEraseMode(false);
                });
            }
        }

        // ── Interaction Prompt ────────────────────────────────────────────

        public void ShowInteractPrompt(string text, bool show)
        {
            interactPromptPanel?.SetActive(show);
            if (interactPromptText != null) interactPromptText.text = text;
        }

        public void OnInteractButton() => _localPlayer?.TryInteract();

        // ── Jump / Emote ──────────────────────────────────────────────────

        public void OnJumpButton() => _localPlayer?.Jump();

        // Call PlayEmote (which goes Command→RpcPlayEmote, not direct RPC from client)
        public void OnEmoteButton() => _localPlayer?.PlayEmote(Random.Range(0, 4));

        // ── Sign Popup ────────────────────────────────────────────────────

        public void ShowSignText(string text)
        {
            signPopup?.SetActive(true);
            if (signContentText != null) signContentText.text = text;
        }

        public void OnCloseSign() => signPopup?.SetActive(false);

        // ── Server Info ───────────────────────────────────────────────────

        public void UpdatePlayerCount(int count)
        {
            if (playerCountText != null)
                playerCountText.text = $"Players: {count}";
        }

        public void OnLeaveButton()
        {
            Multiplayer.SoraNetworkManager.Instance?.LeaveGame();
        }

        // ── Util ──────────────────────────────────────────────────────────

        string CapFirst(string s) =>
            s.Length > 0 ? char.ToUpper(s[0]) + s.Substring(1) : s;
    }
}
