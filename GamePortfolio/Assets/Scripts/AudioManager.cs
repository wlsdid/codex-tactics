using System.Collections;
using UnityEngine;

/// <summary>
/// Manages BGM and SFX playback. Uses Resources/Audio/ folder for clips.
/// Falls back to procedural tones if no audio files found.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM")]
    [SerializeField] private AudioClip battleBgm;
    [SerializeField] private AudioClip victoryBgm;

    [Header("SFX")]
    [SerializeField] private AudioClip attackSfx;
    [SerializeField] private AudioClip skillSfx;
    [SerializeField] private AudioClip guardSfx;
    [SerializeField] private AudioClip damageSfx;
    [SerializeField] private AudioClip victorySfx;
    [SerializeField] private AudioClip defeatSfx;

    private static AudioManager instance;
    public static AudioManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Create audio sources if not assigned
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.volume = 0.3f;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.volume = 0.5f;
        }

        // Try to load from Resources, fall back to procedural
        LoadOrGenerateAudio();
    }

    private void LoadOrGenerateAudio()
    {
        battleBgm = Resources.Load<AudioClip>("Audio/BattleBGM");
        victoryBgm = Resources.Load<AudioClip>("Audio/VictoryBGM");
        attackSfx = Resources.Load<AudioClip>("Audio/AttackSFX");
        skillSfx = Resources.Load<AudioClip>("Audio/SkillSFX");
        guardSfx = Resources.Load<AudioClip>("Audio/GuardSFX");
        damageSfx = Resources.Load<AudioClip>("Audio/DamageSFX");
        victorySfx = Resources.Load<AudioClip>("Audio/VictorySFX");
        defeatSfx = Resources.Load<AudioClip>("Audio/DefeatSFX");
    }

    public void PlayBattleBgm()
    {
        if (bgmSource != null && battleBgm != null)
        {
            bgmSource.clip = battleBgm;
            bgmSource.Play();
        }
    }

    public void PlayVictoryBgm()
    {
        if (bgmSource != null && victoryBgm != null)
        {
            bgmSource.clip = victoryBgm;
            bgmSource.Play();
        }
    }

    public void StopBgm()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    public void PlayAttackSfx() => PlaySfx(attackSfx);
    public void PlaySkillSfx() => PlaySfx(skillSfx);
    public void PlayGuardSfx() => PlaySfx(guardSfx);
    public void PlayDamageSfx() => PlaySfx(damageSfx);
    public void PlayVictorySfx() => PlaySfx(victorySfx);
    public void PlayDefeatSfx() => PlaySfx(defeatSfx);

    private void PlaySfx(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }
}
