using UnityEngine;
using Mirror;

namespace SoraKobo.Player
{
    public class PlayerCustomization : NetworkBehaviour
    {
        [Header("Renderers")]
        public SpriteRenderer bodyRenderer;
        public SpriteRenderer hairRenderer;
        public SpriteRenderer outfitRenderer;
        public SpriteRenderer accessoryRenderer;

        [Header("Sprite Arrays")]
        public Sprite[] hairSprites;
        public Sprite[] outfitSprites;
        public Sprite[] accessorySprites;

        private PlayerController _controller;

        void Awake()
        {
            _controller = GetComponent<PlayerController>();
        }

        public override void OnStartClient()
        {
            ApplyAppearance();
        }

        void ApplyAppearance()
        {
            if (_controller == null) return;

            if (bodyRenderer != null)
                bodyRenderer.color = _controller.skinColor;

            if (hairRenderer != null)
            {
                hairRenderer.color = _controller.hairColor;
                if (hairSprites != null && _controller.hairIndex < hairSprites.Length)
                    hairRenderer.sprite = hairSprites[_controller.hairIndex];
            }

            if (outfitRenderer != null)
            {
                outfitRenderer.color = _controller.outfitColor;
                if (outfitSprites != null && _controller.outfitIndex < outfitSprites.Length)
                    outfitRenderer.sprite = outfitSprites[_controller.outfitIndex];
            }

            if (accessoryRenderer != null)
            {
                if (accessorySprites != null && _controller.accessoryIndex < accessorySprites.Length)
                    accessoryRenderer.sprite = accessorySprites[_controller.accessoryIndex];
                accessoryRenderer.enabled = _controller.accessoryIndex > 0;
            }
        }

        // Called by UI after changes
        public void RefreshAppearance()
        {
            ApplyAppearance();
        }

        void OnValidate()
        {
            // Ensure arrays are initialized in editor
        }
    }
}
