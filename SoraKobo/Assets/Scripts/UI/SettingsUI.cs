using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SoraKobo.UI
{
    public class SettingsUI : MonoBehaviour
    {
        [Header("Audio")]
        public Slider musicVolumeSlider;
        public Slider sfxVolumeSlider;

        [Header("Graphics")]
        public Toggle fullscreenToggle;
        public TMP_Dropdown qualityDropdown;

        [Header("Controls")]
        public Slider joystickSizeSlider;
        public Toggle invertYToggle;

        [Header("Buttons")]
        public Button applyButton;
        public Button backButton;

        void Start()
        {
            // Load saved settings
            musicVolumeSlider?.SetValueWithoutNotify(PlayerPrefs.GetFloat("MusicVolume", 0.4f));
            sfxVolumeSlider?.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFXVolume", 0.8f));

            musicVolumeSlider?.onValueChanged.AddListener(OnMusicVolume);
            sfxVolumeSlider?.onValueChanged.AddListener(OnSFXVolume);

            applyButton?.onClick.AddListener(OnApply);
            backButton?.onClick.AddListener(OnBack);
        }

        void OnMusicVolume(float v) => Audio.AudioManager.Instance?.SetMusicVolume(v);
        void OnSFXVolume(float v)   => Audio.AudioManager.Instance?.SetSFXVolume(v);

        void OnApply()
        {
            PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider?.value ?? 0.4f);
            PlayerPrefs.SetFloat("SFXVolume",   sfxVolumeSlider?.value   ?? 0.8f);
            PlayerPrefs.Save();
        }

        void OnBack() => UIManager.Instance?.ShowScreen("MainMenu");
    }
}
