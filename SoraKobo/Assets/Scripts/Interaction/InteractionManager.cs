using UnityEngine;
using System.Collections.Generic;

namespace SoraKobo.Interaction
{
    /// <summary>
    /// Scene-level manager for all interactable objects.
    /// Provides a registry and proximity query used by the HUD.
    /// </summary>
    public class InteractionManager : MonoBehaviour
    {
        public static InteractionManager Instance { get; private set; }

        private List<InteractableObject> _allInteractables = new List<InteractableObject>();

        void Awake() => Instance = this;

        public void Register(InteractableObject obj)
        {
            if (!_allInteractables.Contains(obj))
                _allInteractables.Add(obj);
        }

        public void Unregister(InteractableObject obj)
        {
            _allInteractables.Remove(obj);
        }

        /// <summary>Returns the nearest interactable within range, or null.</summary>
        public InteractableObject GetNearest(Vector2 position, float range)
        {
            InteractableObject nearest = null;
            float minDist = range;
            foreach (var obj in _allInteractables)
            {
                if (obj == null) continue;
                float d = Vector2.Distance(position, obj.transform.position);
                if (d < minDist) { minDist = d; nearest = obj; }
            }
            return nearest;
        }
    }
}
