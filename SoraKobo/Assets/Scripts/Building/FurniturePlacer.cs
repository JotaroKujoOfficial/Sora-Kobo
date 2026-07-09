using UnityEngine;
using Mirror;

namespace SoraKobo.Building
{
    /// <summary>
    /// Allows players to place pre-built furniture/interactable prefabs into the world.
    /// These are separate from blocks — they are full GameObject prefabs with scripts.
    /// </summary>
    public class FurniturePlacer : NetworkBehaviour
    {
        [Header("Furniture Prefabs")]
        public GameObject[] furniturePrefabs; // Chair, Piano, Door, Swing, etc.
        public int selectedFurnitureIndex = 0;

        private Camera _cam;

        void Start() => _cam = Camera.main;

        public void PlaceFurnitureAt(Vector2 screenPos)
        {
            if (!isLocalPlayer) return;
            if (furniturePrefabs == null || furniturePrefabs.Length == 0) return;
            if (selectedFurnitureIndex >= furniturePrefabs.Length) return;

            Vector3 world = _cam.ScreenToWorldPoint(
                new Vector3(screenPos.x, screenPos.y, -_cam.transform.position.z));
            world.z = 0;

            // Snap to grid
            world.x = Mathf.Round(world.x);
            world.y = Mathf.Round(world.y);

            CmdPlaceFurniture(selectedFurnitureIndex, world);
        }

        [Command]
        void CmdPlaceFurniture(int prefabIndex, Vector3 position)
        {
            if (prefabIndex >= furniturePrefabs.Length) return;
            var go = Instantiate(furniturePrefabs[prefabIndex], position, Quaternion.identity);
            NetworkServer.Spawn(go);
        }

        public void SelectFurniture(int index) => selectedFurnitureIndex = index;
    }
}
