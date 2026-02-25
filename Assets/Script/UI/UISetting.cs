using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class UISetting : MonoBehaviour
{
    [Header("Audio Settings")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public TextMeshProUGUI musicVolTxt;
    public TextMeshProUGUI sfxVolTxt;

    [Header("Control Settings")]
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityTxt;
    public Toggle hapticsToggle;

    [Header("Graphics UI")]
    public Button[] qualityButtons;
    public Color selectedBtnColor = new Color(0f, 0.8f, 1f, 1f);
    public Color normalBtnColor = new Color(0.3f, 0.3f, 0.3f, 1f); 

    private void Start()
    {
        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
            OnMusicVolTextChanged();
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
            OnSFXVolTextChanged();
        }
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 1.5f); // Đổi mặc định thành 1.5f
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            OnSensitivityChanged(sensitivitySlider.value);
        }

        if (hapticsToggle != null)
        {
            bool isHapticsOn = PlayerPrefs.GetInt("Haptics", 1) == 1;
            hapticsToggle.isOn = isHapticsOn;
            hapticsToggle.onValueChanged.AddListener(OnHapticsChanged);
        }


        int defaultGraphicLevel = 2;

        int savedQuality = PlayerPrefs.GetInt("QualityLevel", defaultGraphicLevel);
        SetGraphicsQuality(savedQuality);
    }


    private void OnMusicSliderChanged(float value)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusicVolume(value);
        OnMusicVolTextChanged();
    }

    private void OnSFXSliderChanged(float value)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetSFXVolume(value);
        OnSFXVolTextChanged();
    }

    private void OnMusicVolTextChanged()
    {
        if (musicVolTxt != null && musicSlider != null)
            musicVolTxt.text = $"{Mathf.RoundToInt(musicSlider.value * 100)}%";
    }

    private void OnSFXVolTextChanged()
    {
        if (sfxVolTxt != null && sfxSlider != null)
            sfxVolTxt.text = $"{Mathf.RoundToInt(sfxSlider.value * 100)}%";
    }

    private void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        PlayerPrefs.Save();

        if (sensitivityTxt != null)
        {
            sensitivityTxt.text = $"x{value.ToString("F1")}";
        }

        if (PlayerController.Instance != null) PlayerController.Instance.UpdateSensitivity();
    }

    private void OnHapticsChanged(bool isOn)
    {
        PlayerPrefs.SetInt("Haptics", isOn ? 1 : 0);
        PlayerPrefs.Save();

        if (isOn)
        {
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        }
    }

    // ---------------- GRAPHICS HANDLERS ----------------
    public void SetGraphicsQuality(int qualityIndex)
    {
        int maxIndex = QualitySettings.names.Length - 1;
        int safeIndex = Mathf.Clamp(qualityIndex, 0, maxIndex);

        QualitySettings.SetQualityLevel(safeIndex);
        PlayerPrefs.SetInt("QualityLevel", safeIndex);
        PlayerPrefs.Save();

        UpdateGraphicsButtonsUI(safeIndex);

    }

    private void UpdateGraphicsButtonsUI(int selectedIndex)
    {
        if (qualityButtons == null || qualityButtons.Length == 0) return;

        for (int i = 0; i < qualityButtons.Length; i++)
        {
            if (qualityButtons[i] != null)
            {
                Image btnImage = qualityButtons[i].GetComponent<Image>();
                if (btnImage != null)
                {
                    btnImage.color = (i == selectedIndex) ? selectedBtnColor : normalBtnColor;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSFXSliderChanged);
        if (sensitivitySlider != null) sensitivitySlider.onValueChanged.RemoveListener(OnSensitivityChanged);
        if (hapticsToggle != null) hapticsToggle.onValueChanged.RemoveListener(OnHapticsChanged);
    }
}