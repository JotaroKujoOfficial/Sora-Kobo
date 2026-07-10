using UnityEngine;
using Mirror;

namespace SoraKobo.Interaction
{
    public class ChairObject : InteractableObject
    {
        [Header("Chair")]
        public Transform sitAnchor;

        [SyncVar] private bool _occupied;
        [SyncVar] private uint _sitterNetId;

        void Start()
        {
            interactType = InteractableType.Chair;
            interactPrompt = "Sit down";
            if (sitAnchor == null) sitAnchor = transform;
        }

        public override void Interact(Player.PlayerController player)
        {
            if (!player.isLocalPlayer) return;

            if (_sitterNetId == player.netId)
            {
                CmdStandUp(player.netId);
                player.enabled = true;
            }
            else if (!_occupied)
            {
                CmdSit(player.netId);
                player.transform.position = sitAnchor.position;
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                player.enabled = false;
            }
        }

        [Command(requiresAuthority = false)]
        void CmdSit(uint netId)
        {
            _occupied    = true;
            _sitterNetId = netId;
        }

        [Command(requiresAuthority = false)]
        void CmdStandUp(uint netId)
        {
            if (_sitterNetId != netId) return;
            _occupied    = false;
            _sitterNetId = 0;
        }
    }
}
