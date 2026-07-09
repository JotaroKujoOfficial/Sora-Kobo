using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SoraKobo.UI
{
    public class PianoUI : MonoBehaviour
    {
        [Header("Panel")]
        public GameObject pianoPanel;

        [Header("Keys (8 buttons)")]
        public Button[] keyButtons;
        public string[] noteNames = { "Do", "Re", "Mi", "Fa", "Sol", "La", "Si", "Do'" };

        private Interaction.PianoObject _currentPiano;

        void Start()
        {
            pianoPanel?.SetActive(false);
            for (int i = 0; i < keyButtons.Length; i++)
            {
                int captured = i;
                keyButtons[i]?.onClick.AddListener(() => OnKeyPress(captured));

                // Label
                var txt = keyButtons[i]?.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null && captured < noteNames.Length)
                    txt.text = noteNames[captured];
            }
        }

        public void Open(Interaction.PianoObject piano)
        {
            _currentPiano = piano;
            pianoPanel?.SetActive(true);
        }

        public void Close()
        {
            pianoPanel?.SetActive(false);
            _currentPiano = null;
        }

        void OnKeyPress(int index)
        {
            if (_currentPiano != null)
                _currentPiano.CmdPressKey(index);
            else
                Audio.AudioManager.Instance?.PlayNote(index);
        }
    }
}
