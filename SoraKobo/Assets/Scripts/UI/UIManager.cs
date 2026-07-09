using UnityEngine;
using System.Collections.Generic;

namespace SoraKobo.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Screens")]
        public GameObject mainMenuScreen;
        public GameObject gameScreen;
        public GameObject lobbyScreen;
        public GameObject customizationScreen;
        public GameObject mapEditorScreen;

        private Dictionary<string, GameObject> _screens;
        private string _currentScreen;

        void Awake()
        {
            Instance = this;
            _screens = new Dictionary<string, GameObject>
            {
                { "MainMenu",      mainMenuScreen      },
                { "Game",          gameScreen          },
                { "Lobby",         lobbyScreen         },
                { "Customization", customizationScreen },
                { "MapEditor",     mapEditorScreen     },
            };
        }

        void OnEnable()
        {
            // Subscribe to network events without a direct assembly dependency on Multiplayer
            Multiplayer.SoraNetworkManager.OnScreenRequested   += ShowScreen;
            Multiplayer.SoraNetworkManager.OnPlayerCountChanged += UpdatePlayerCount;
        }

        void OnDisable()
        {
            Multiplayer.SoraNetworkManager.OnScreenRequested   -= ShowScreen;
            Multiplayer.SoraNetworkManager.OnPlayerCountChanged -= UpdatePlayerCount;
        }

        void Start() => ShowScreen("MainMenu");

        public void ShowScreen(string name)
        {
            foreach (var kvp in _screens)
                if (kvp.Value != null)
                    kvp.Value.SetActive(kvp.Key == name);
            _currentScreen = name;
        }

        public void UpdatePlayerCount(int count)
        {
            var hud = FindObjectOfType<HUDController>();
            hud?.UpdatePlayerCount(count);
        }
    }
}
