using UnityEngine;

namespace SoraKobo.Multiplayer
{
    /// <summary>
    /// Single entry point. Place on a persistent GameObject in the Bootstrap scene.
    /// Loads the MainMenu scene on start.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        public static GameBootstrap Instance { get; private set; }

        [Header("App Info")]
        public string gameName    = "Sora Kobo";
        public string gameVersion = "1.0.0";

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // Android back button
            Input.backButtonLeavesApp = false;

            InitSystems();
        }

        void InitSystems()
        {
            // Pre-generate piano notes if AudioManager is available
            var audio = FindObjectOfType<Audio.AudioManager>();
            if (audio != null && (audio.pianoNotes == null || audio.pianoNotes.Length == 0))
                audio.GeneratePianoNotes();
        }

        void Update()
        {
            // Android back button handling
            if (Input.GetKeyDown(KeyCode.Escape))
                HandleBackButton();
        }

        void HandleBackButton()
        {
            var nm = SoraNetworkManager.Instance;
            if (nm == null) return;

            if (Mirror.NetworkClient.isConnected)
            {
                nm.LeaveGame();
            }
        }

        public static string GetPlatformInfo()
        {
            return $"{SystemInfo.deviceModel} | {SystemInfo.operatingSystem} | {Screen.width}x{Screen.height}";
        }
    }
}
