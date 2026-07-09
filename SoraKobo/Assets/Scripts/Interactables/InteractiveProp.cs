using UnityEngine;
using Mirror;

namespace SoraKobo.Interactables
{
    /// <summary>
    /// Generic animated prop. Plays an animation and optionally a sound on interact.
    /// Use for decorative interactables: fans, TV, lamps, etc.
    /// </summary>
    public class InteractiveProp : Interaction.InteractableObject
    {
        [Header("Prop Settings")]
        public string animTrigger = "Toggle";
        public bool toggleState = false;

        [SyncVar(hook = nameof(OnStateChanged))]
        private bool _active = false;

        void Start()
        {
            interactType = Interaction.InteractableType.Decoration;
            interactPrompt = "Interact";
        }

        public override void Interact(Player.PlayerController player)
        {
            if (!player.isLocalPlayer) return;
            CmdToggle();
        }

        [Command(requiresAuthority = false)]
        void CmdToggle() => _active = !_active;

        void OnStateChanged(bool old, bool val)
        {
            var anim = GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetBool("Active", val);
                if (!string.IsNullOrEmpty(animTrigger)) anim.SetTrigger(animTrigger);
            }
            Audio.AudioManager.Instance?.PlayInteract();
        }
    }
}
