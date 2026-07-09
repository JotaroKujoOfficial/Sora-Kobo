using Mirror;
using UnityEngine;

namespace SoraKobo.Multiplayer.NetworkMessages
{
    public struct MapDataMessage : NetworkMessage
    {
        public string json;
    }

    public struct PlayerJoinedMessage : NetworkMessage
    {
        public uint   netId;
        public string playerName;
    }

    public struct PlayerLeftMessage : NetworkMessage
    {
        public uint netId;
    }

    public struct ChatMessage : NetworkMessage
    {
        public string sender;
        public string content;
    }

    public struct EmoteMessage : NetworkMessage
    {
        public uint senderNetId;
        public int  emoteId;
    }

    /// <summary>
    /// Register client-side message handlers once per session.
    /// Call from SoraNetworkManager.OnStartClient.
    /// </summary>
    public static class Handlers
    {
        public static void RegisterClient()
        {
            NetworkClient.RegisterHandler<MapDataMessage>(OnMapData);
        }

        static void OnMapData(MapDataMessage msg)
        {
            if (string.IsNullOrEmpty(msg.json)) return;
            var building = Object.FindObjectOfType<Building.BuildingSystem>();
            if (building != null && NetworkClient.active && !NetworkServer.active)
                building.ServerLoadMap(msg.json);
        }
    }
}
