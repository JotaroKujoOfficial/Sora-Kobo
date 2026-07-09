using UnityEngine;
using System.Collections.Generic;

namespace SoraKobo.Data
{
    [System.Serializable]
    public class BlockData
    {
        public string blockId;
        public string blockName;
        public Color color;
        public int spriteIndex;
        public bool isSolid;
        public bool isInteractable;
    }

    [System.Serializable]
    public class PlacedBlockData
    {
        public string blockId;
        public int x;
        public int y;
        public int layer; // 0 = background, 1 = main, 2 = foreground
    }

    [System.Serializable]
    public class MapSaveData
    {
        public string mapName;
        public string authorName;
        public int width;
        public int height;
        public List<PlacedBlockData> blocks = new List<PlacedBlockData>();
        public string saveDate;
    }

    [System.Serializable]
    public class PlayerSaveData
    {
        public string playerName;
        public int hairIndex;
        public int outfitIndex;
        public int accessoryIndex;
        public Color hairColor;
        public Color skinColor;
        public Color outfitColor;
    }

    [CreateAssetMenu(fileName = "BlockDatabase", menuName = "SoraKobo/Block Database")]
    public class BlockDatabase : ScriptableObject
    {
        public List<BlockData> blocks = new List<BlockData>();

        private Dictionary<string, BlockData> _lookup;

        public BlockData GetById(string id)
        {
            if (_lookup == null) BuildLookup();
            _lookup.TryGetValue(id, out var data);
            return data;
        }

        void BuildLookup()
        {
            _lookup = new Dictionary<string, BlockData>();
            foreach (var b in blocks)
                _lookup[b.blockId] = b;
        }
    }
}
