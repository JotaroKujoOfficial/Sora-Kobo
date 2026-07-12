using UnityEngine;
using UnityEngine.UI;

namespace SoraKobo.UI
{
    /// <summary>
    /// Simple minimap — renders a RenderTexture view of the world.
    /// Place a secondary UnityEngine.Camera above the scene, set its CullingMask and
    /// Target Texture to a RenderTexture, then assign it here.
    /// </summary>
    public class MiniMap : MonoBehaviour
    {
        [Header("Mini Map UnityEngine.Camera")]
        public UnityEngine.Camera miniMapCamera;
        public float cameraHeight = 30f;
        public RawImage displayImage;

        [Header("Player Dot")]
        public RectTransform playerDot;
        public float worldSize = 80f; // match grid width

        private Transform _playerTransform;

        void Start()
        {
            if (miniMapCamera != null)
            {
                miniMapCamera.orthographic = true;
                miniMapCamera.orthographicSize = worldSize * 0.5f;
            }
        }

        void Update()
        {
            if (_playerTransform == null)
            {
                var p = FindLocalPlayer();
                if (p != null) _playerTransform = p.transform;
            }

            FollowPlayer();
            UpdateDot();
        }

        void FollowPlayer()
        {
            if (miniMapCamera == null || _playerTransform == null) return;
            miniMapCamera.transform.position = new Vector3(
                _playerTransform.position.x,
                cameraHeight,
                _playerTransform.position.z);
        }

        void UpdateDot()
        {
            if (playerDot == null || _playerTransform == null) return;

            // Map world position to minimap UV
            float u = _playerTransform.position.x / worldSize;
            float v = _playerTransform.position.y / worldSize;

            if (displayImage != null)
            {
                var rect = displayImage.rectTransform.rect;
                playerDot.anchoredPosition = new Vector2(
                    (u - 0.5f) * rect.width,
                    (v - 0.5f) * rect.height);
            }
        }

        Player.PlayerController FindLocalPlayer()
        {
            foreach (var p in FindObjectsOfType<Player.PlayerController>())
                if (p.isLocalPlayer) return p;
            return null;
        }
    }
}
