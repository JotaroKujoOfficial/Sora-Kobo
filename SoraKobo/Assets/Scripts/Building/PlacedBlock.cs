using UnityEngine;
using Mirror;
using System.Collections.Generic;

namespace SoraKobo.Building
{
    public class PlacedBlock : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnBlockIdChanged))]
        public string blockId = "grass";

        [Header("Visuals")]
        public SpriteRenderer spriteRenderer;

        private static readonly Dictionary<string, Color> BlockColors
            = new Dictionary<string, Color>
        {
            { "grass",  new Color(0.33f, 0.73f, 0.33f) },
            { "dirt",   new Color(0.60f, 0.40f, 0.20f) },
            { "stone",  new Color(0.55f, 0.55f, 0.55f) },
            { "wood",   new Color(0.55f, 0.35f, 0.15f) },
            { "sand",   new Color(0.93f, 0.87f, 0.60f) },
            { "water",  new Color(0.25f, 0.55f, 0.93f) },
            { "snow",   new Color(0.93f, 0.96f, 1.00f) },
            { "brick",  new Color(0.75f, 0.30f, 0.20f) },
            { "glass",  new Color(0.75f, 0.90f, 1.00f, 0.5f) },
            { "planks", new Color(0.72f, 0.52f, 0.25f) },
            { "leaf",   new Color(0.20f, 0.60f, 0.25f) },
            { "sky",    new Color(0.53f, 0.80f, 0.92f, 0.3f) },
            { "cloud",  new Color(1f,    1f,    1f,    0.7f) },
            { "flower", new Color(1.0f,  0.50f, 0.70f) },
        };

        public override void OnStartClient() => ApplyVisual(blockId);

        void OnBlockIdChanged(string _, string newVal) => ApplyVisual(newVal);

        void ApplyVisual(string id)
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return;
            spriteRenderer.color = BlockColors.TryGetValue(id, out var col) ? col : Color.white;
        }

        // ── Static helpers ────────────────────────────────────────────────

        public static Color GetColor(string id) =>
            BlockColors.TryGetValue(id, out var col) ? col : Color.white;

        public static List<string> GetAllBlockIds() =>
            new List<string>(BlockColors.Keys);

        public static bool IsKnownBlock(string id) =>
            !string.IsNullOrEmpty(id) && BlockColors.ContainsKey(id);
    }
}
