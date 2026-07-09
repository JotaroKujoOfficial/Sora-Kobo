using UnityEngine;
using Mirror;

namespace SoraKobo.Multiplayer
{
    /// <summary>
    /// Syncs additional per-frame player state: animation, emotes, sitting flag.
    /// Kept separate from PlayerController for clarity.
    /// </summary>
    public class PlayerSyncExtras : NetworkBehaviour
    {
        [SyncVar] public bool isSitting = false;
        [SyncVar] public int  lastEmote = -1;
        [SyncVar] public float posX     = 0f;
        [SyncVar] public float posY     = 0f;

        private Player.PlayerController _ctrl;
        private float _sendInterval = 0.05f; // 20 Hz
        private float _sendTimer = 0f;

        void Awake() => _ctrl = GetComponent<Player.PlayerController>();

        void Update()
        {
            if (!isLocalPlayer) return;
            _sendTimer += Time.deltaTime;
            if (_sendTimer < _sendInterval) return;
            _sendTimer = 0f;

            CmdSyncPosition(transform.position.x, transform.position.y);
        }

        [Command]
        void CmdSyncPosition(float x, float y)
        {
            posX = x;
            posY = y;
        }

        void LateUpdate()
        {
            // Smoothly interpolate remote players
            if (isLocalPlayer) return;
            Vector3 target = new Vector3(posX, posY, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, target, 10f * Time.deltaTime);
        }
    }
}
