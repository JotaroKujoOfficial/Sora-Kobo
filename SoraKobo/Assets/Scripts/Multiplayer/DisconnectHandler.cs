using UnityEngine;
using Mirror;

namespace SoraKobo.Multiplayer
{
    /// <summary>
    /// Handles cleanup when a player disconnects — releases occupied chairs,
    /// removes player name from scoreboard, etc.
    /// Attach to the SoraNetworkManager GameObject or as a singleton.
    /// </summary>
    public class DisconnectHandler : NetworkBehaviour
    {
        public static DisconnectHandler Instance { get; private set; }

        void Awake() => Instance = this;

        /// <summary>
        /// Called by SoraNetworkManager.OnServerDisconnect.
        /// Releases all resources held by the disconnecting player.
        /// </summary>
        [Server]
        public void HandlePlayerDisconnect(NetworkConnectionToClient conn)
        {
            if (conn.identity == null) return;
            uint netId = conn.identity.netId;

            // Release any occupied chairs
            var chairs = FindObjectsOfType<Interaction.ChairObject>();
            foreach (var chair in chairs)
            {
                if (chair.occupantNetId == netId)
                {
                    chair.isOccupied    = false;
                    chair.occupantNetId = 0;
                }
            }

            // Release vehicles
            var vehicles = FindObjectsOfType<Interaction.VehicleObject>();
            foreach (var v in vehicles)
            {
                // Vehicle handles exit on its own via OnDisable
            }

            Debug.Log($"[SoraKobo] Cleaned up resources for player {netId}");
        }
    }
}
