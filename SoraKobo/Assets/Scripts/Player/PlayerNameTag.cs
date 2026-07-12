using UnityEngine;
using TMPro;

namespace SoraKobo.Player
{
    public class PlayerNameTag : MonoBehaviour
    {
        public TextMeshPro nameText;
        private UnityEngine.Camera _mainCam;

        void Start()
        {
            _mainCam = UnityEngine.UnityEngine.Camera.main;
        }

        void LateUpdate()
        {
            // Always face camera
            if (_mainCam != null)
                transform.rotation = _mainCam.transform.rotation;
        }

        public void SetName(string name)
        {
            if (nameText != null) nameText.text = name;
        }
    }
}
