using UnityEngine;

namespace SoraKobo.Interactables
{
    public class SignObject : MonoBehaviour
    {
        [TextArea(2, 5)]
        public string signText = "Hello, World!";
    }
}
