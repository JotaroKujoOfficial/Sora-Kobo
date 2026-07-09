using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System.Collections.Generic;

namespace SoraKobo.UI
{
    public class ChatUI : NetworkBehaviour
    {
        [Header("Chat UI")]
        public GameObject chatPanel;
        public ScrollRect scrollRect;
        public Transform messageContainer;
        public GameObject messagePrefab;
        public TMP_InputField inputField;
        public Button sendButton;
        public Button toggleButton;

        private bool _isOpen = false;
        private List<string> _messageHistory = new List<string>();

        void Start()
        {
            chatPanel?.SetActive(false);
            sendButton?.onClick.AddListener(OnSend);
        }

        public void ToggleChat()
        {
            _isOpen = !_isOpen;
            chatPanel?.SetActive(_isOpen);
            if (_isOpen && inputField != null) inputField.Select();
        }

        public void OnSend()
        {
            if (inputField == null || string.IsNullOrWhiteSpace(inputField.text)) return;
            string msg = inputField.text.Trim();
            inputField.text = "";

            var player = GetLocalPlayer();
            string sender = player != null ? player.playerName : "Player";
            CmdSendMessage(sender, msg);
        }

        [Command(requiresAuthority = false)]
        void CmdSendMessage(string sender, string message)
        {
            RpcReceiveMessage(sender, message);
        }

        [ClientRpc]
        void RpcReceiveMessage(string sender, string message)
        {
            AddMessage($"<b>{sender}:</b> {message}");
        }

        void AddMessage(string text)
        {
            _messageHistory.Add(text);
            if (_messageHistory.Count > 100) _messageHistory.RemoveAt(0);

            if (messagePrefab != null && messageContainer != null)
            {
                var go = Instantiate(messagePrefab, messageContainer);
                var tmp = go.GetComponent<TextMeshProUGUI>();
                if (tmp != null) tmp.text = text;
            }

            // Auto-scroll
            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        Player.PlayerController GetLocalPlayer()
        {
            foreach (var p in FindObjectsOfType<Player.PlayerController>())
                if (p.isLocalPlayer) return p;
            return null;
        }
    }
}
