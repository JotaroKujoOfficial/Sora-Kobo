using UnityEngine;
using Mirror;

namespace SoraKobo.Building
{
    /// <summary>
    /// Server-side procedural world generator.
    /// Called once when hosting a new game with no saved map.
    /// </summary>
    public class WorldGenerator : NetworkBehaviour
    {
        [Header("World Parameters")]
        public int width        = 80;
        public int groundY      = 6;
        public int dirtDepth    = 4;
        public float hillAmplitude = 2f;
        public float hillFreq      = 0.08f;
        public int treeSpacing     = 6;
        public int treeMinHeight   = 3;
        public int treeMaxHeight   = 6;

        private BuildingSystem _building;

        void Start() => _building = BuildingSystem.Instance;

        [Server]
        public void Generate()
        {
            if (_building == null) return;

            for (int x = 0; x < width; x++)
            {
                // Undulating terrain using Perlin noise
                int surfaceY = groundY + Mathf.RoundToInt(
                    Mathf.PerlinNoise(x * hillFreq, 0f) * hillAmplitude);

                // Sky background
                for (int y = surfaceY + 1; y < 35; y++)
                    _building.CmdPlaceBlock("sky", new Vector2Int(x, y), 0);

                // Ground surface
                _building.CmdPlaceBlock("grass", new Vector2Int(x, surfaceY), 1);

                // Dirt below
                for (int y = surfaceY - 1; y >= Mathf.Max(0, surfaceY - dirtDepth); y--)
                    _building.CmdPlaceBlock("dirt", new Vector2Int(x, y), 1);

                // Stone deeper
                for (int y = surfaceY - dirtDepth - 1; y >= 0; y--)
                    _building.CmdPlaceBlock("stone", new Vector2Int(x, y), 1);

                // Trees
                if (x % treeSpacing == 0 && x > 0 && x < width - treeSpacing)
                {
                    int h = Random.Range(treeMinHeight, treeMaxHeight + 1);
                    for (int ty = surfaceY + 1; ty <= surfaceY + h; ty++)
                        _building.CmdPlaceBlock("wood", new Vector2Int(x, ty), 1);
                    // Leaf crown
                    for (int lx = x - 2; lx <= x + 2; lx++)
                        for (int ly = surfaceY + h; ly <= surfaceY + h + 2; ly++)
                            _building.CmdPlaceBlock("leaf", new Vector2Int(lx, ly), 1);
                }

                // Flowers occasionally
                if (Random.value < 0.1f)
                    _building.CmdPlaceBlock("flower", new Vector2Int(x, surfaceY + 1), 2);
            }

            // Add clouds
            for (int i = 0; i < 8; i++)
            {
                int cx = Random.Range(5, width - 5);
                int cy = Random.Range(22, 30);
                for (int dx = -2; dx <= 2; dx++)
                    _building.CmdPlaceBlock("cloud", new Vector2Int(cx + dx, cy), 2);
                _building.CmdPlaceBlock("cloud", new Vector2Int(cx - 1, cy + 1), 2);
                _building.CmdPlaceBlock("cloud", new Vector2Int(cx,     cy + 1), 2);
                _building.CmdPlaceBlock("cloud", new Vector2Int(cx + 1, cy + 1), 2);
            }
        }
    }
}
