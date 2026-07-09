using UnityEngine;
using System.Collections.Generic;
using SoraKobo.Data;

namespace SoraKobo.Building
{
    /// <summary>
    /// Extension helpers for BuildingSystem.
    /// Provides fill, line, and copy-paste operations.
    /// </summary>
    public static class BuildingSystemHelper
    {
        // ── Flood fill ────────────────────────────────────────────────────

        public static List<Vector2Int> FloodFill(
            Dictionary<Vector2Int, string> grid,
            Vector2Int start,
            int maxWidth,
            int maxHeight,
            int fillLimit = 200)
        {
            var result = new List<Vector2Int>();
            var visited = new HashSet<Vector2Int>();
            var queue = new Queue<Vector2Int>();

            string targetId = grid.ContainsKey(start) ? grid[start] : null;

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0 && result.Count < fillLimit)
            {
                var pos = queue.Dequeue();
                result.Add(pos);

                foreach (var dir in new Vector2Int[] {
                    Vector2Int.up, Vector2Int.down,
                    Vector2Int.left, Vector2Int.right })
                {
                    var next = pos + dir;
                    if (visited.Contains(next)) continue;
                    if (next.x < 0 || next.x >= maxWidth || next.y < 0 || next.y >= maxHeight) continue;

                    string nextId = grid.ContainsKey(next) ? grid[next] : null;
                    if (nextId != targetId) continue;

                    visited.Add(next);
                    queue.Enqueue(next);
                }
            }
            return result;
        }

        // ── Draw line (Bresenham) ─────────────────────────────────────────

        public static List<Vector2Int> GetLine(Vector2Int from, Vector2Int to)
        {
            var result = new List<Vector2Int>();
            int x = from.x, y = from.y;
            int dx = Mathf.Abs(to.x - from.x), sx = from.x < to.x ? 1 : -1;
            int dy = -Mathf.Abs(to.y - from.y), sy = from.y < to.y ? 1 : -1;
            int err = dx + dy;

            while (true)
            {
                result.Add(new Vector2Int(x, y));
                if (x == to.x && y == to.y) break;
                int e2 = 2 * err;
                if (e2 >= dy) { err += dy; x += sx; }
                if (e2 <= dx) { err += dx; y += sy; }
            }
            return result;
        }

        // ── Rectangle fill ────────────────────────────────────────────────

        public static List<Vector2Int> GetRectangle(Vector2Int corner1, Vector2Int corner2, bool filled = false)
        {
            var result = new List<Vector2Int>();
            int x0 = Mathf.Min(corner1.x, corner2.x);
            int x1 = Mathf.Max(corner1.x, corner2.x);
            int y0 = Mathf.Min(corner1.y, corner2.y);
            int y1 = Mathf.Max(corner1.y, corner2.y);

            for (int x = x0; x <= x1; x++)
            {
                for (int y = y0; y <= y1; y++)
                {
                    bool isBorder = x == x0 || x == x1 || y == y0 || y == y1;
                    if (filled || isBorder)
                        result.Add(new Vector2Int(x, y));
                }
            }
            return result;
        }

        // ── Copy region ───────────────────────────────────────────────────

        public static List<PlacedBlockData> CopyRegion(
            Dictionary<Vector2Int, string> grid,
            Vector2Int min, Vector2Int max, int layer)
        {
            var result = new List<PlacedBlockData>();
            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (grid.ContainsKey(pos))
                        result.Add(new PlacedBlockData
                        {
                            blockId = grid[pos],
                            x = x - min.x,
                            y = y - min.y,
                            layer = layer
                        });
                }
            }
            return result;
        }
    }
}
