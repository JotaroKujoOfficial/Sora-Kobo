using UnityEngine;
using System.IO;
using SoraKobo.Data;

namespace SoraKobo.Building
{
    /// <summary>
    /// Utility: read/write MapSaveData to disk in JSON format.
    /// Works on both Android (persistentDataPath) and desktop.
    /// </summary>
    public static class MapSerializer
    {
        static string GetDir()
        {
            string dir = Path.Combine(Application.persistentDataPath, "Maps");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return dir;
        }

        public static void Save(MapSaveData data)
        {
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            string path = Path.Combine(GetDir(), data.mapName + ".json");
            File.WriteAllText(path, json);
            Debug.Log($"[SoraKobo] Map saved → {path}");
        }

        public static MapSaveData Load(string mapName)
        {
            string path = Path.Combine(GetDir(), mapName + ".json");
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[SoraKobo] Map not found: {path}");
                return null;
            }
            return JsonUtility.FromJson<MapSaveData>(File.ReadAllText(path));
        }

        public static string[] ListMaps()
        {
            string dir = GetDir();
            var files = Directory.GetFiles(dir, "*.json");
            var names = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
                names[i] = Path.GetFileNameWithoutExtension(files[i]);
            return names;
        }

        public static void Delete(string mapName)
        {
            string path = Path.Combine(GetDir(), mapName + ".json");
            if (File.Exists(path)) File.Delete(path);
        }

        public static string ToJson(MapSaveData data) => JsonUtility.ToJson(data);
        public static MapSaveData FromJson(string json) => JsonUtility.FromJson<MapSaveData>(json);
    }
}
