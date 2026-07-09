using UnityEngine;
using Mirror;

namespace SoraKobo.Interaction
{
    public class SwingObject : InteractableObject
    {
        [Header("Swing Settings")]
        public float swingAmplitude = 30f;
        public float swingSpeed = 2f;

        [SyncVar] private bool _swinging = false;
        private float _swingTime = 0f;
        private Quaternion _startRot;

        void Start()
        {
            _startRot = transform.localRotation;
            interactType = InteractableType.Swing;
            interactPrompt = "Tap to swing!";
        }

        void Update()
        {
            if (!_swinging) return;
            _swingTime += Time.deltaTime;
            float angle = swingAmplitude * Mathf.Sin(_swingTime * swingSpeed);
            transform.localRotation = _startRot * Quaternion.Euler(0, 0, angle);
        }

        public override void Interact(Player.PlayerController player)
        {
            if (!player.isLocalPlayer) return;
            CmdToggleSwing();
        }

        [Command(requiresAuthority = false)]
        void CmdToggleSwing() => _swinging = !_swinging;
    }
}
