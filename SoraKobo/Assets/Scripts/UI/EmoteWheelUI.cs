using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SoraKobo.UI
{
    public class EmoteWheelUI : MonoBehaviour
    {
        [Header("Wheel")]
        public GameObject emoteWheelPanel;
        public Button[]   emoteButtons;
        public string[]   emoteNames = { "Wave", "Dance", "Sit", "Jump" };
        public Sprite[]   emoteIcons;

        private Player.PlayerController _localPlayer;
        private bool _isOpen;

        void Start()
        {
            emoteWheelPanel?.SetActive(false);
            for (int i = 0; i < emoteButtons.Length; i++)
            {
                int captured = i;
                var btn = emoteButtons[i];
                if (btn == null) continue;
                btn.onClick.AddListener(() => OnEmote(captured));
                var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null && captured < emoteNames.Length) txt.text = emoteNames[captured];
                if (emoteIcons != null && captured < emoteIcons.Length)
                {
                    var img = btn.GetComponent<Image>();
                    if (img != null) img.sprite = emoteIcons[captured];
                }
            }
        }

        public void ToggleWheel()
        {
            _isOpen = !_isOpen;
            emoteWheelPanel?.SetActive(_isOpen);
            if (_isOpen && _localPlayer == null) _localPlayer = FindLocalPlayer();
        }

        void OnEmote(int index)
        {
            if (_localPlayer == null) _localPlayer = FindLocalPlayer();
            // PlayEmote goes through Command → ClientRpc (correct direction)
            _localPlayer?.PlayEmote(index);
            emoteWheelPanel?.SetActive(false);
            _isOpen = false;
        }

        Player.PlayerController FindLocalPlayer()
        {
            foreach (var p in FindObjectsOfType<Player.PlayerController>())
                if (p.isLocalPlayer) return p;
            return null;
        }
    }
}
