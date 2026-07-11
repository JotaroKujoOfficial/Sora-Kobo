using UnityEngine;
using Mirror;
using UnityEngine.Events;
using SoraKobo.Player;

namespace SoraKobo.Interaction
{
    public enum InteractableType
    {
        Chair, Piano, Door, Swing, Food, Vehicle, Decoration, Sign
    }

    public class InteractableObject : NetworkBehaviour
    {
        [Header("Interaction")]
        public InteractableType interactType;
        public string interactPrompt = "Interact";
        public Transform sitPoint;

        [SyncVar] public bool isOccupied    = false;
        [SyncVar] public uint occupantNetId = 0;

        // ── Events — avoids a hard reference to HUDController ─────────────
        // HUDController subscribes at runtime via OnLocalPlayerSpawned.
        public static event System.Action<string, bool> OnPromptChanged;

        protected Player.PlayerController _currentPlayer;

        // ── Public interface ──────────────────────────────────────────────

        public virtual void Interact(Player.PlayerController player)
        {
            if (!player.isLocalPlayer) return;
            switch (interactType)
            {
                case InteractableType.Chair:      TrySit(player);        break;
                case InteractableType.Piano:      TryPlayPiano(player);  break;
                case InteractableType.Door:       TryToggleDoor();       break;
                case InteractableType.Food:       TryEat(player);        break;
                default:                          CmdGenericInteract();  break;
            }
        }

        // ── Sit ───────────────────────────────────────────────────────────

        protected virtual void TrySit(Player.PlayerController player)
        {
            if (isOccupied && occupantNetId != player.netId) return;

            if (occupantNetId == player.netId)
            {
                CmdSetOccupied(false, 0);
                player.enabled = true;
            }
            else
            {
                CmdSetOccupied(true, player.netId);
                if (sitPoint != null) player.transform.position = sitPoint.position;
                var rb2d = player.GetComponent<Rigidbody2D>();
                if (rb2d != null) rb2d.velocity = Vector2.zero;
                _currentPlayer = player;
            }
        }

        // ── Piano ─────────────────────────────────────────────────────────

        protected virtual void TryPlayPiano(Player.PlayerController player)
        {
            CmdPlayNote(Random.Range(0, 8));
        }

        [Command(requiresAuthority = false)]
        protected void CmdPlayNote(int noteIndex) => RpcPlayNote(noteIndex);

        [ClientRpc]
        protected void RpcPlayNote(int noteIndex) =>
            Audio.AudioManager.Instance?.PlayNote(noteIndex);

        // ── Door ──────────────────────────────────────────────────────────

        protected virtual void TryToggleDoor() => CmdToggleDoor();

        [Command(requiresAuthority = false)]
        protected void CmdToggleDoor() => RpcToggleDoor();

        [ClientRpc]
        protected void RpcToggleDoor()
        {
            GetComponent<Animator>()?.SetTrigger("Toggle");
            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = !col.enabled;
        }

        // ── Food ──────────────────────────────────────────────────────────

        protected virtual void TryEat(Player.PlayerController player) =>
            CmdEat(player.netId);

        [Command(requiresAuthority = false)]
        protected void CmdEat(uint playerId)
        {
            RpcShowEatEffect();
            NetworkServer.Destroy(gameObject);
        }

        [ClientRpc]
        protected void RpcShowEatEffect() =>
            Audio.AudioManager.Instance?.PlayInteract();

        // ── Generic ───────────────────────────────────────────────────────

        [Command(requiresAuthority = false)]
        protected void CmdGenericInteract() => RpcGenericInteract();

        [ClientRpc]
        protected void RpcGenericInteract() =>
            GetComponent<Animator>()?.SetTrigger("Interact");

        [Command(requiresAuthority = false)]
        protected void CmdSetOccupied(bool occupied, uint netId)
        {
            isOccupied    = occupied;
            occupantNetId = netId;
        }

        // ── Proximity trigger — fires event instead of touching HUD ───────

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (IsLocalPlayerCollider(other))
                OnPromptChanged?.Invoke(interactPrompt, true);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (IsLocalPlayerCollider(other))
                OnPromptChanged?.Invoke("", false);
        }

        bool IsLocalPlayerCollider(Collider2D col)
        {
            var p = col.GetComponent<Player.PlayerController>();
            return p != null && p.isLocalPlayer;
        }
    }
}
