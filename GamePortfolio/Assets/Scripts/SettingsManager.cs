using UnityEngine;

/// <summary>
/// Persistent settings manager for audio volume preferences.
/// Saves/loads to PlayerPrefs. Accessible globally via Singleton.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    private const string BgmVolumeKey = "Settings_BgmVolume";
    private const string SfxVolumeKey = "Settings_SfxVolume";

    private static SettingsManager instance;
    public static SettingsManager Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("SettingsManager");
                instance = go.AddComponent<SettingsManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    [Range(0f, 1f)] [SerializeField] private float bgmVolume = 0.3f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 0.5f;

    public float BgmVolume
    {
        get => bgmVolume;
        set
        {
            bgmVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(BgmVolumeKey, bgmVolume);
            PlayerPrefs.Save();
            ApplyVolumes();
        }
    }

    public float SfxVolume
    {
        get => sfxVolume;
        set
        {
            sfxVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolume);
            PlayerPrefs.Save();
            ApplyVolumes();
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved preferences
        bgmVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(BgmVolumeKey, 0.3f));
        sfxVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(SfxVolumeKey, 0.5f));

        ApplyVolumes();
    }

    /// <summary>Reset all settings to defaults.</summary>
    public void ResetToDefaults()
    {
        bgmVolume = 0.3f;
        sfxVolume = 0.5f;
        PlayerPrefs.SetFloat(BgmVolumeKey, bgmVolume);
        PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolume);
        PlayerPrefs.Save();
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetBgmVolume(bgmVolume);
            AudioManager.Instance.SetSfxVolume(sfxVolume);
        }
    }
}
