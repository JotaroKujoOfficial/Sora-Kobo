using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Mirror;

namespace SoraKobo.UI
{
    public class PlayerListUI : MonoBehaviour
    {
        [Header("UI")]
        public GameObject playerListPanel;
        public Transform listContainer;
        public GameObject playerEntryPrefab;
        public TextMeshProUGUI titleText;

        private bool _isOpen = false;

        void Start()
        {
            playerListPanel?.SetActive(false);
        }

        public void TogglePanel()
        {
            _isOpen = !_isOpen;
            playerListPanel?.SetActive(_isOpen);
            if (_isOpen) RefreshList();
        }

        void RefreshList()
        {
            if (listContainer == null) return;
            foreach (Transform t in listContainer) Destroy(t.gameObject);

            var players = FindObjectsOfType<Player.PlayerController>();
            if (titleText != null) titleText.text = $"Players ({players.Length}/10)";

            foreach (var p in players)
            {
                if (playerEntryPrefab == null) continue;
                var go = Instantiate(playerEntryPrefab, listContainer);
                var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
                if (texts.Length > 0) texts[0].text = p.playerName;
                if (texts.Length > 1) texts[1].text = p.isLocalPlayer ? "(You)" : "";
            }
        }

        void Update()
        {
            if (_isOpen) RefreshList(); // Live update
        }
    }
}
