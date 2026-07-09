using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

namespace SoraKobo.UI
{
    public class CharacterCustomizationUI : MonoBehaviour
    {
        [Header("Preview")]
        public SpriteRenderer previewBody;
        public SpriteRenderer previewHair;
        public SpriteRenderer previewOutfit;
        public SpriteRenderer previewAccessory;

        [Header("Name")]
        public TMP_InputField playerNameInput;

        [Header("Hair")]
        public Button hairPrevButton;
        public Button hairNextButton;
        public TextMeshProUGUI hairIndexText;
        public Image hairColorPreview;
        public Slider hairHueSlider;

        [Header("Outfit")]
        public Button outfitPrevButton;
        public Button outfitNextButton;
        public TextMeshProUGUI outfitIndexText;
        public Image outfitColorPreview;
        public Slider outfitHueSlider;

        [Header("Skin")]
        public Slider skinSlider;
        public Image skinColorPreview;

        [Header("Accessory")]
        public Button accPrevButton;
        public Button accNextButton;
        public TextMeshProUGUI accIndexText;

        [Header("Buttons")]
        public Button saveButton;
        public Button backButton;

        // Working state
        private string _playerName = "Player";
        private int _hairIndex = 0;
        private int _outfitIndex = 0;
        private int _accIndex = 0;
        private float _hairHue = 0.15f;
        private float _outfitHue = 0.6f;
        private float _skinTone = 0.8f;

        private int _maxHair = 5;
        private int _maxOutfit = 6;
        private int _maxAcc = 4;

        void OnEnable()
        {
            LoadFromPrefs();
            RefreshPreview();
        }

        void LoadFromPrefs()
        {
            string json = PlayerPrefs.GetString("PlayerSave", "");
            if (!string.IsNullOrEmpty(json))
            {
                var data = JsonUtility.FromJson<Data.PlayerSaveData>(json);
                _playerName   = data.playerName;
                _hairIndex    = data.hairIndex;
                _outfitIndex  = data.outfitIndex;
                _accIndex     = data.accessoryIndex;
                Color.RGBToHSV(data.hairColor,   out _hairHue,   out _, out _);
                Color.RGBToHSV(data.outfitColor,  out _outfitHue, out _, out _);
                Color.RGBToHSV(data.skinColor,    out _,          out _, out _skinTone);
            }

            if (playerNameInput != null) playerNameInput.text = _playerName;
            if (hairHueSlider   != null) hairHueSlider.value   = _hairHue;
            if (outfitHueSlider != null) outfitHueSlider.value = _outfitHue;
            if (skinSlider      != null) skinSlider.value      = _skinTone;
        }

        // ── Hair ──────────────────────────────────────────────────────────
        public void OnHairPrev() { _hairIndex = (_hairIndex - 1 + _maxHair) % _maxHair; RefreshPreview(); }
        public void OnHairNext() { _hairIndex = (_hairIndex + 1)            % _maxHair; RefreshPreview(); }
        public void OnHairHueChanged(float val) { _hairHue = val; RefreshPreview(); }

        // ── Outfit ────────────────────────────────────────────────────────
        public void OnOutfitPrev() { _outfitIndex = (_outfitIndex - 1 + _maxOutfit) % _maxOutfit; RefreshPreview(); }
        public void OnOutfitNext() { _outfitIndex = (_outfitIndex + 1)              % _maxOutfit; RefreshPreview(); }
        public void OnOutfitHueChanged(float val) { _outfitHue = val; RefreshPreview(); }

        // ── Skin ──────────────────────────────────────────────────────────
        public void OnSkinChanged(float val) { _skinTone = val; RefreshPreview(); }

        // ── Accessory ─────────────────────────────────────────────────────
        public void OnAccPrev() { _accIndex = (_accIndex - 1 + _maxAcc) % _maxAcc; RefreshPreview(); }
        public void OnAccNext() { _accIndex = (_accIndex + 1)           % _maxAcc; RefreshPreview(); }

        void RefreshPreview()
        {
            Color hairCol   = Color.HSVToRGB(_hairHue,   0.7f, 0.85f);
            Color outfitCol = Color.HSVToRGB(_outfitHue, 0.7f, 0.85f);
            Color skinCol   = Color.Lerp(new Color(0.95f, 0.75f, 0.55f), new Color(0.2f, 0.12f, 0.07f), 1f - _skinTone);

            if (previewHair   != null) previewHair.color   = hairCol;
            if (previewOutfit != null) previewOutfit.color = outfitCol;
            if (previewBody   != null) previewBody.color   = skinCol;

            if (hairColorPreview   != null) hairColorPreview.color   = hairCol;
            if (outfitColorPreview != null) outfitColorPreview.color = outfitCol;
            if (skinColorPreview   != null) skinColorPreview.color   = skinCol;

            if (hairIndexText  != null) hairIndexText.text  = $"Hair {_hairIndex + 1}/{_maxHair}";
            if (outfitIndexText != null) outfitIndexText.text = $"Outfit {_outfitIndex + 1}/{_maxOutfit}";
            if (accIndexText   != null) accIndexText.text   = $"Acc {_accIndex + 1}/{_maxAcc}";
        }

        public void OnSave()
        {
            if (playerNameInput != null) _playerName = playerNameInput.text.Trim();
            if (string.IsNullOrEmpty(_playerName)) _playerName = "Player";

            Color hairCol   = Color.HSVToRGB(_hairHue,   0.7f, 0.85f);
            Color outfitCol = Color.HSVToRGB(_outfitHue, 0.7f, 0.85f);
            Color skinCol   = Color.Lerp(new Color(0.95f, 0.75f, 0.55f), new Color(0.2f, 0.12f, 0.07f), 1f - _skinTone);

            var data = new Data.PlayerSaveData
            {
                playerName    = _playerName,
                hairIndex     = _hairIndex,
                outfitIndex   = _outfitIndex,
                accessoryIndex = _accIndex,
                hairColor     = hairCol,
                skinColor     = skinCol,
                outfitColor   = outfitCol
            };

            PlayerPrefs.SetString("PlayerSave", JsonUtility.ToJson(data));
            PlayerPrefs.SetString("PlayerName", _playerName);
            PlayerPrefs.Save();

            // Apply to live player if in game
            var player = FindObjectOfType<Player.PlayerController>();
            if (player != null && player.isLocalPlayer)
                player.CmdSetAppearance(_playerName, _hairIndex, _outfitIndex, _accIndex,
                    hairCol, skinCol, outfitCol);

            OnBack();
        }

        public void OnBack()
        {
            UIManager.Instance?.ShowScreen("MainMenu");
        }
    }
}
