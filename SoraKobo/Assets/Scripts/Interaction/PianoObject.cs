using UnityEngine;
using Mirror;
using SoraKobo.Player;

namespace SoraKobo.Interaction
{
    public class PianoObject : InteractableObject
    {
        [Header("Piano Keys")]
        public GameObject[] keyObjects; // 8 key GameObjects for visual feedback

        void Start()
        {
            interactType = InteractableType.Piano;
            interactPrompt = "Play piano ♪";
        }

        public override void Interact(Player.PlayerController player)
        {
            if (!player.isLocalPlayer) return;
            // Open piano mini-UI
            var pianoUI = FindObjectOfType<UI.PianoUI>();
            if (pianoUI != null) pianoUI.Open(this);
        }

        [Command(requiresAuthority = false)]
        public void CmdPressKey(int noteIndex)
        {
            RpcPressKey(noteIndex);
        }

        [ClientRpc]
        void RpcPressKey(int noteIndex)
        {
            Audio.AudioManager.Instance?.PlayNote(noteIndex);
            AnimateKey(noteIndex);
        }

        void AnimateKey(int index)
        {
            if (keyObjects == null || index >= keyObjects.Length) return;
            var key = keyObjects[index];
            if (key == null) return;
            StartCoroutine(PressAnimation(key));
        }

        System.Collections.IEnumerator PressAnimation(GameObject key)
        {
            var origPos = key.transform.localPosition;
            key.transform.localPosition += Vector3.down * 0.05f;
            yield return new WaitForSeconds(0.1f);
            key.transform.localPosition = origPos;
        }
    }
}
