using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown crosshairTypeDropdown;
    public Slider crosshairSizeSlider;
    public TMP_Dropdown crosshairColorDropdown;
    public Slider fovSlider;
    public TMP_InputField fovInputField;
    public Slider mouseSensitivitySlider;
    public TMP_InputField mouseSensitivityInputField;
    public Toggle muteToggle;
    public Slider volumeSlider;
    public GameObject settingsPanel;
    public GameObject mainMenuPanel;

    private void Start()
    {
        LoadSettings();
        AddInputListeners();
        settingsPanel.SetActive(false); // Hide initially
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        SaveSettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("CrosshairType", crosshairTypeDropdown.value);
        PlayerPrefs.SetFloat("CrosshairSize", crosshairSizeSlider.value);
        PlayerPrefs.SetInt("CrosshairColor", crosshairColorDropdown.value);
        PlayerPrefs.SetFloat("FOV", fovSlider.value);
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivitySlider.value);
        PlayerPrefs.SetInt("MuteAudio", muteToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("MasterVolume", volumeSlider.value);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        crosshairTypeDropdown.value = PlayerPrefs.GetInt("CrosshairType", 0);
        crosshairSizeSlider.value = PlayerPrefs.GetFloat("CrosshairSize", 1f);
        crosshairColorDropdown.value = PlayerPrefs.GetInt("CrosshairColor", 0);

        float savedFOV = PlayerPrefs.GetFloat("FOV", 90f);
        fovSlider.value = savedFOV;
        fovInputField.text = savedFOV.ToString("F1");

        float savedSens = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        mouseSensitivitySlider.value = savedSens;
        mouseSensitivityInputField.text = savedSens.ToString("F2");

        muteToggle.isOn = PlayerPrefs.GetInt("MuteAudio", 0) == 1;
        volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 10f);
    }

    private void AddInputListeners()
    {
        // FOV slider → input
        fovSlider.onValueChanged.AddListener((value) =>
        {
            fovInputField.text = value.ToString("F1");
        });

        // FOV input → slider
        fovInputField.onEndEdit.AddListener((value) =>
        {
            if (float.TryParse(value, out float fovVal))
            {
                fovSlider.value = fovVal;
            }
        });

        // Sensitivity slider → input
        mouseSensitivitySlider.onValueChanged.AddListener((value) =>
        {
            mouseSensitivityInputField.text = value.ToString("F2");
        });

        // Sensitivity input → slider
        mouseSensitivityInputField.onEndEdit.AddListener((value) =>
        {
            if (float.TryParse(value, out float sensVal))
            {
                mouseSensitivitySlider.value = sensVal;
            }
        });
    }
}