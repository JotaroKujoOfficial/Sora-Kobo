using UnityEngine;
using Mirror;
using SoraKobo.Player;

namespace SoraKobo.Interaction
{
    public class FoodObject : InteractableObject
    {
        [Header("Food Settings")]
        public string foodName = "Apple";
        public Sprite foodSprite;

        void Start()
        {
            interactType = InteractableType.Food;
            interactPrompt = $"Eat {foodName}";
        }

        public override void Interact(Player.PlayerController player)
        {
            if (!player.isLocalPlayer) return;
            CmdEatFood(player.netId);
        }

        [Command(requiresAuthority = false)]
        void CmdEatFood(uint playerId)
        {
            RpcEatEffect();
            // Destroy food item after eaten
            NetworkServer.Destroy(gameObject);
        }

        [ClientRpc]
        void RpcEatEffect()
        {
            // Play eat sound
            Audio.AudioManager.Instance?.PlayInteract();
        }
    }
}
