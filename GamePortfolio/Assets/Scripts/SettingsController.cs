using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runtime controller for the Settings scene.
/// Wires up slider changes to SettingsManager and button actions.
/// </summary>
public class SettingsController : MonoBehaviour
{
    [Header("Sliders")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("Buttons")]
    public Button backButton;
    public Button resetDataButton;

    [Header("Labels")]
    [SerializeField] private TMP_Text bgmValueLabel;
    [SerializeField] private TMP_Text sfxValueLabel;

    /// <summary>Setter used by batch scene builder.</summary>
    public void TrySetBgmValueLabel(TMP_Text label) => bgmValueLabel = label;
    /// <summary>Setter used by batch scene builder.</summary>
    public void TrySetSfxValueLabel(TMP_Text label) => sfxValueLabel = label;

    private void Start()
    {
        // Init slider values from saved settings
        if (bgmSlider != null && SettingsManager.Instance != null)
        {
            bgmSlider.value = SettingsManager.Instance.BgmVolume;
            bgmSlider.onValueChanged.AddListener(OnBgmChanged);
        }

        if (sfxSlider != null && SettingsManager.Instance != null)
        {
            sfxSlider.value = SettingsManager.Instance.SfxVolume;
            sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        }

        // Wire buttons
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);

        if (resetDataButton != null)
            resetDataButton.onClick.AddListener(OnResetDataClicked);

        // Init value labels
        if (bgmValueLabel != null)
            bgmValueLabel.text = Mathf.RoundToInt((bgmSlider?.value ?? 0.3f) * 100) + "%";

        if (sfxValueLabel != null)
            sfxValueLabel.text = Mathf.RoundToInt((sfxSlider?.value ?? 0.5f) * 100) + "%";

        // Play title BGM in settings
        if (AudioManager.Instance != null)
            AudioManager.Instance.CrossfadeTitle();
    }

    private void OnBgmChanged(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.BgmVolume = value;

        if (bgmValueLabel != null)
            bgmValueLabel.text = Mathf.RoundToInt(value * 100) + "%";
    }

    private void OnSfxChanged(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.SfxVolume = value;

        if (sfxValueLabel != null)
            sfxValueLabel.text = Mathf.RoundToInt(value * 100) + "%";

        // Play a preview SFX so user can hear the change
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayItemSfx();
    }

    private void OnBackClicked()
    {
        // Return to title screen
        var flow = FindFirstObjectByType<GameSceneFlow>();
        if (flow != null)
            flow.LoadTitle();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
    }

    private void OnResetDataClicked()
    {
        // Reset save data
        SaveManager.DeleteSave();

        // Reset settings to defaults
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.ResetToDefaults();

        if (bgmSlider != null)
            bgmSlider.value = 0.3f;

        if (sfxSlider != null)
            sfxSlider.value = 0.5f;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDefeatSfx();

        Debug.Log("Save data and settings have been reset.");
    }
}
