using UnityEngine;
using UnityEngine.EventSystems;

namespace SoraKobo.Building
{
    /// <summary>
    /// Translates touch/click on the world canvas into build placement calls.
    /// Attach to a full-screen transparent RawImage that sits above the world
    /// but below the HUD buttons.
    /// </summary>
    public class BuildTouchInput : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        private BuildingSystem _system;

        void Start() => _system = BuildingSystem.Instance;

        public void OnPointerDown(PointerEventData e) => TryBuild(e.position);
        public void OnDrag(PointerEventData e)        => TryBuild(e.position);

        void TryBuild(Vector2 screenPos)
        {
            // BuildingSystem is a scene NetworkBehaviour; isLocalPlayer is always false.
            // Guard instead on NetworkClient.active so touch input only fires on clients.
            if (_system == null || !Mirror.NetworkClient.active) return;
            _system.OnTapBuild(screenPos);
            Audio.AudioManager.Instance?.PlayBlockPlace();
        }
    }
}
