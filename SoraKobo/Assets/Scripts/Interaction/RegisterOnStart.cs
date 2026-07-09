using UnityEngine;

namespace SoraKobo.Interaction
{
    /// <summary>Auto-registers the attached InteractableObject with the InteractionManager.</summary>
    [RequireComponent(typeof(InteractableObject))]
    public class RegisterOnStart : MonoBehaviour
    {
        void Start()
        {
            var obj = GetComponent<InteractableObject>();
            InteractionManager.Instance?.Register(obj);
        }

        void OnDestroy()
        {
            var obj = GetComponent<InteractableObject>();
            InteractionManager.Instance?.Unregister(obj);
        }
    }
}
