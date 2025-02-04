using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    public GameObject settingsPanel;
    public TMP_Dropdown qualityDropdown; 
    public Toggle fullscreenToggle;
    public Slider volumeSlider;

    private void Start()
    {
        InitializeSettings();
    }

    private void InitializeSettings()
    {
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.onValueChanged.AddListener(SetQuality);

        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void ToggleSettingsMenu()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    private void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    private void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void UnToggleSettingsMenu()
    {
        settingsPanel.SetActive(false);
    }
}