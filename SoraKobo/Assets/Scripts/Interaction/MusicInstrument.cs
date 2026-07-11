using UnityEngine;
using Mirror;
using SoraKobo.Player;

namespace SoraKobo.Interaction
{
    public enum InstrumentType { Piano, Guitar, Drum, Xylophone }

    public class MusicInstrument : InteractableObject
    {
        [Header("Instrument")]
        public InstrumentType instrumentType;
        public int noteCount = 8;

        void Start()
        {
            interactType = InteractableType.Piano;
            interactPrompt = $"Play {instrumentType}";
        }

        public override void Interact(Player.PlayerController player)
        {
            if (!player.isLocalPlayer) return;
            var pianoUI = FindObjectOfType<UI.PianoUI>();
            if (pianoUI != null) pianoUI.Open(GetComponent<PianoObject>());
        }

        [Command(requiresAuthority = false)]
        public void CmdPlayNote(int noteIdx)
        {
            RpcPlayNote(noteIdx);
        }

        [ClientRpc]
        void RpcPlayNote(int noteIdx)
        {
            int offset = (int)instrumentType * 2;
            Audio.AudioManager.Instance?.PlayNote((noteIdx + offset) % 8);
        }
    }
}
