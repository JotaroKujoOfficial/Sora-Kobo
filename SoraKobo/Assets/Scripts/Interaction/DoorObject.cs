using UnityEngine;
using Mirror;

namespace SoraKobo.Interaction
{
    public class DoorObject : InteractableObject
    {
        [Header("Door Settings")]
        public bool startOpen = false;

        [SyncVar(hook = nameof(OnOpenChanged))]
        private bool _isOpen;

        private Collider2D _col;
        private Animator   _anim;

        void Start()
        {
            interactType = InteractableType.Door;
            interactPrompt = "Open / Close door";
            _col  = GetComponent<Collider2D>();
            _anim = GetComponent<Animator>();
            _isOpen = startOpen;
            ApplyState(_isOpen);
        }

        public override void Interact(Player.PlayerController player)
        {
            if (!player.isLocalPlayer) return;
            CmdToggle();
        }

        [Command(requiresAuthority = false)]
        void CmdToggle() => _isOpen = !_isOpen;

        void OnOpenChanged(bool oldVal, bool newVal) => ApplyState(newVal);

        void ApplyState(bool open)
        {
            if (_col != null) _col.enabled = !open;
            if (_anim != null) _anim.SetBool("Open", open);
            else
            {
                // Fallback: rotate door
                transform.localRotation = open
                    ? Quaternion.Euler(0, 0, 90f)
                    : Quaternion.identity;
            }
        }
    }
}
