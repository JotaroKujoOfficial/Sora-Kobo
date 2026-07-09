using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace SoraKobo.UI
{
    public class ConfirmDialog : MonoBehaviour
    {
        public static ConfirmDialog Instance { get; private set; }

        [Header("UI")]
        public GameObject panel;
        public TextMeshProUGUI questionText;
        public Button confirmButton;
        public Button cancelButton;

        private Action _onConfirm;
        private Action _onCancel;

        void Awake()
        {
            Instance = this;
            panel?.SetActive(false);
            confirmButton?.onClick.AddListener(OnConfirm);
            cancelButton?.onClick.AddListener(OnCancel);
        }

        public static void Ask(string question, Action onYes, Action onNo = null)
        {
            Instance?.Show(question, onYes, onNo);
        }

        public void Show(string question, Action onYes, Action onNo = null)
        {
            _onConfirm = onYes;
            _onCancel  = onNo;
            if (questionText != null) questionText.text = question;
            panel?.SetActive(true);
        }

        void OnConfirm()
        {
            panel?.SetActive(false);
            _onConfirm?.Invoke();
        }

        void OnCancel()
        {
            panel?.SetActive(false);
            _onCancel?.Invoke();
        }
    }
}
