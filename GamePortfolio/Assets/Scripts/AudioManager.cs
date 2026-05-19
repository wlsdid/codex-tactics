using System.Collections;
using UnityEngine;

/// <summary>
/// Manages BGM and SFX playback.
/// Generates procedural audio tones if no Resources/Audio/ clips found.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM")]
    [SerializeField] private AudioClip battleBgm;
    [SerializeField] private AudioClip victoryBgm;
    [SerializeField] private AudioClip titleBgm;
    [SerializeField] private AudioClip stageSelectBgm;

    [Header("SFX")]
    [SerializeField] private AudioClip attackSfx;
    [SerializeField] private AudioClip skillSfx;
    [SerializeField] private AudioClip guardSfx;
    [SerializeField] private AudioClip damageSfx;
    [SerializeField] private AudioClip victorySfx;
    [SerializeField] private AudioClip defeatSfx;
    [SerializeField] private AudioClip itemSfx;
    [SerializeField] private AudioClip breakSfx;
    [SerializeField] private AudioClip levelUpSfx;
    [SerializeField] private AudioClip healSfx;
    [SerializeField] private AudioClip shieldSfx;
    [SerializeField] private AudioClip stunSfx;
    [SerializeField] private AudioClip burnSfx;

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

        LoadOrGenerateAudio();

        // Apply any persisted settings volume after loading
        if (SettingsManager.Instance != null)
        {
            SetBgmVolume(SettingsManager.Instance.BgmVolume);
            SetSfxVolume(SettingsManager.Instance.SfxVolume);
        }
    }

    private void LoadOrGenerateAudio()
    {
        battleBgm = Resources.Load<AudioClip>("Audio/BattleBGM") ?? GenerateProceduralBgm(0.3f);
        victoryBgm = Resources.Load<AudioClip>("Audio/VictoryBGM") ?? GenerateVictoryTone();
        titleBgm = Resources.Load<AudioClip>("Audio/TitleBGM") ?? GenerateTitleBgm(0.25f);
        stageSelectBgm = Resources.Load<AudioClip>("Audio/StageSelectBGM") ?? GenerateStageSelectBgm(0.25f);
        attackSfx = Resources.Load<AudioClip>("Audio/AttackSFX") ?? GenerateTone(220f, 0.12f, 0.3f);
        skillSfx = Resources.Load<AudioClip>("Audio/SkillSFX") ?? GenerateTone(440f, 0.18f, 0.4f);
        guardSfx = Resources.Load<AudioClip>("Audio/GuardSFX") ?? GenerateTone(180f, 0.15f, 0.25f);
        damageSfx = Resources.Load<AudioClip>("Audio/DamageSFX") ?? GenerateTone(120f, 0.12f, 0.35f);
        victorySfx = Resources.Load<AudioClip>("Audio/VictorySFX") ?? GenerateVictoryChime();
        defeatSfx = Resources.Load<AudioClip>("Audio/DefeatSFX") ?? GenerateTone(80f, 0.3f, 0.3f);
        itemSfx = Resources.Load<AudioClip>("Audio/ItemSFX") ?? GenerateTone(520f, 0.15f, 0.2f);
        breakSfx = Resources.Load<AudioClip>("Audio/BreakSFX") ?? GenerateBreakTone();
        levelUpSfx = Resources.Load<AudioClip>("Audio/LevelUpSFX") ?? GenerateLevelUpChime();
        healSfx = Resources.Load<AudioClip>("Audio/HealSFX") ?? GenerateTone(660f, 0.2f, 0.25f);
        shieldSfx = Resources.Load<AudioClip>("Audio/ShieldSFX") ?? GenerateTone(350f, 0.18f, 0.2f);
        stunSfx = Resources.Load<AudioClip>("Audio/StunSFX") ?? GenerateTone(100f, 0.2f, 0.15f);
        burnSfx = Resources.Load<AudioClip>("Audio/BurnSFX") ?? GenerateTone(200f, 0.25f, 0.15f);
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

    public void PlayTitleBgm()
    {
        if (bgmSource != null && titleBgm != null)
        {
            bgmSource.clip = titleBgm;
            bgmSource.Play();
        }
    }

    public void PlayStageSelectBgm()
    {
        if (bgmSource != null && stageSelectBgm != null)
        {
            bgmSource.clip = stageSelectBgm;
            bgmSource.Play();
        }
    }

    public void StopBgm() { if (bgmSource != null) bgmSource.Stop(); }

    public void SetBgmVolume(float volume)
    {
        if (bgmSource != null)
            bgmSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSfxVolume(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = Mathf.Clamp01(volume);
    }

    /// <summary>Crossfade to a new BGM over the specified duration.</summary>
    public void CrossfadeTo(AudioClip newClip, float fadeDuration = 1.0f)
    {
        if (newClip == null) return;
        StopAllCoroutines();
        StartCoroutine(CrossfadeRoutine(newClip, fadeDuration));
    }

    public void CrossfadeBattle() { if (battleBgm != null) CrossfadeTo(battleBgm); }
    public void CrossfadeVictory() { if (victoryBgm != null) CrossfadeTo(victoryBgm); }
    public void CrossfadeTitle() { if (titleBgm != null) CrossfadeTo(titleBgm); }
    public void CrossfadeStageSelect() { if (stageSelectBgm != null) CrossfadeTo(stageSelectBgm); }

    private IEnumerator CrossfadeRoutine(AudioClip newClip, float duration)
    {
        if (bgmSource == null) yield break;

        // Fade out current
        float startVolume = bgmSource.volume;
        float elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (duration * 0.5f));
            yield return null;
        }
        bgmSource.Stop();

        // Switch clip
        bgmSource.clip = newClip;
        bgmSource.Play();

        // Fade in
        elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, startVolume, elapsed / (duration * 0.5f));
            yield return null;
        }
        bgmSource.volume = startVolume;
    }

    public void PlayAttackSfx() => PlaySfx(attackSfx);
    public void PlaySkillSfx() => PlaySfx(skillSfx);
    public void PlayGuardSfx() => PlaySfx(guardSfx);
    public void PlayDamageSfx() => PlaySfx(damageSfx);
    public void PlayVictorySfx() => PlaySfx(victorySfx);
    public void PlayDefeatSfx() => PlaySfx(defeatSfx);
    public void PlayItemSfx() => PlaySfx(itemSfx);
    public void PlayBreakSfx() => PlaySfx(breakSfx);
    public void PlayLevelUpSfx() => PlaySfx(levelUpSfx);
    public void PlayHealSfx() => PlaySfx(healSfx);
    public void PlayShieldSfx() => PlaySfx(shieldSfx);
    public void PlayStunSfx() => PlaySfx(stunSfx);
    public void PlayBurnSfx() => PlaySfx(burnSfx);

    private void PlaySfx(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // ── Procedural audio generation ──

    private static AudioClip GenerateTone(float freq, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = Mathf.Max(1, Mathf.RoundToInt(sampleRate * duration));
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = Mathf.Exp(-t * 8f / Mathf.Max(0.01f, duration));
            data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * volume * envelope;
        }
        AudioClip clip = AudioClip.Create("ProceduralTone", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private static AudioClip GenerateVictoryChime()
    {
        int sampleRate = 44100;
        float duration = 0.5f;
        int samples = Mathf.RoundToInt(sampleRate * duration);
        float[] data = new float[samples];
        float[] freqs = { 523f, 659f, 784f, 1047f }; // C5, E5, G5, C6
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float env = Mathf.Exp(-t * 3f);
            float val = 0f;
            for (int j = 0; j < freqs.Length; j++)
            {
                float noteStart = j * 0.08f;
                float localT = t - noteStart;
                if (localT > 0f && localT < 0.3f)
                {
                    float noteEnv = Mathf.Exp(-localT * 6f);
                    val += Mathf.Sin(2f * Mathf.PI * freqs[j] * t) * noteEnv;
                }
            }
            data[i] = val * 0.3f * env;
        }
        AudioClip clip = AudioClip.Create("VictoryChime", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private static AudioClip GenerateBreakTone()
    {
        int sampleRate = 44100;
        float duration = 0.3f;
        int samples = Mathf.RoundToInt(sampleRate * duration);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float freq = Mathf.Lerp(800f, 200f, t / duration);
            float envelope = Mathf.Exp(-t * 4f);
            data[i] = (Mathf.Sin(2f * Mathf.PI * freq * t) + Mathf.Sin(2f * Mathf.PI * freq * 1.5f * t) * 0.5f) * 0.3f * envelope;
        }
        AudioClip clip = AudioClip.Create("BreakTone", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private static AudioClip GenerateLevelUpChime()
    {
        int sampleRate = 44100;
        float duration = 0.6f;
        int samples = Mathf.RoundToInt(sampleRate * duration);
        float[] data = new float[samples];
        float[] freqs = { 392f, 523f, 659f, 784f };
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float env = Mathf.Exp(-t * 2f);
            float val = 0f;
            for (int j = 0; j < freqs.Length; j++)
            {
                float noteStart = j * 0.06f;
                float localT = t - noteStart;
                if (localT > 0f && localT < 0.5f)
                {
                    float noteEnv = Mathf.Exp(-localT * 3f);
                    val += Mathf.Sin(2f * Mathf.PI * freqs[j] * t) * noteEnv * 0.6f;
                }
            }
            data[i] = val * env * 0.3f;
        }
        AudioClip clip = AudioClip.Create("LevelUpChime", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private static AudioClip GenerateVictoryTone()
    {
        int sampleRate = 44100;
        float duration = 2f;
        int samples = Mathf.RoundToInt(sampleRate * duration);
        float[] data = new float[samples];
        float[] melody = { 262f, 294f, 330f, 349f, 392f, 440f, 494f, 523f };
        float beatDuration = duration / melody.Length;
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            int noteIndex = Mathf.Min(Mathf.FloorToInt(t / beatDuration), melody.Length - 1);
            float noteT = (t - noteIndex * beatDuration) / beatDuration;
            float env = Mathf.Exp(-noteT * 3f) * 0.5f;
            data[i] = Mathf.Sin(2f * Mathf.PI * melody[noteIndex] * t) * env;
        }
        AudioClip clip = AudioClip.Create("VictoryMelody", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private static AudioClip GenerateProceduralBgm(float volume)
    {
        int sampleRate = 44100;
        float duration = 4f;
        int samples = Mathf.RoundToInt(sampleRate * duration);
        float[] data = new float[samples];
        float bpm = 120f;
        float beat = 60f / bpm;
        float[] notes = { 130.81f, 146.83f, 164.81f, 174.61f, 196f, 220f, 246.94f, 261.63f };
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            int noteIndex = Mathf.FloorToInt(t / beat) % notes.Length;
            float noteT = (t - Mathf.FloorToInt(t / beat) * beat) / beat;
            float env = Mathf.Exp(-noteT * 3f) * 0.4f;
            float val = Mathf.Sin(2f * Mathf.PI * notes[noteIndex] * t) * env;
            // Add subtle harmony
            val += Mathf.Sin(2f * Mathf.PI * notes[noteIndex] * 1.5f * t) * env * 0.2f;
            data[i] = val * volume;
        }
        AudioClip clip = AudioClip.Create("ProceduralBGM", samples, 1, sampleRate, true);
        clip.SetData(data, 0);
        return clip;
    }

    private static AudioClip GenerateTitleBgm(float volume)
    {
        int sampleRate = 44100;
        float duration = 6f;
        int samples = Mathf.RoundToInt(sampleRate * duration);
        float[] data = new float[samples];
        // Ambient, slow arpeggio — Cmaj7 feel: C3, E3, G3, B3, C4
        float[] chord = { 130.81f, 164.81f, 196f, 246.94f, 261.63f };
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float val = 0f;
            for (int j = 0; j < chord.Length; j++)
            {
                float phaseOffset = (float)j / chord.Length * 2f;
                float arp = Mathf.Sin(2f * Mathf.PI * chord[j] * (t + phaseOffset));
                val += arp * (0.12f + 0.08f * Mathf.Sin(t * 0.5f));
            }
            // Soft pad with subtle vibrato
            float pad = Mathf.Sin(2f * Mathf.PI * 65.41f * t + Mathf.Sin(t * 3f) * 0.02f) * 0.15f;
            data[i] = (val * 0.20f + pad) * volume;
        }
        AudioClip clip = AudioClip.Create("TitleBGM", samples, 1, sampleRate, true);
        clip.SetData(data, 0);
        return clip;
    }

    private static AudioClip GenerateStageSelectBgm(float volume)
    {
        int sampleRate = 44100;
        float duration = 5f;
        int samples = Mathf.RoundToInt(sampleRate * duration);
        float[] data = new float[samples];
        // Determined, march-like progression
        float[] melody = { 196f, 220f, 261.63f, 293.66f, 329.63f, 392f, 329.63f, 293.66f };
        float beat = 60f / 100f;
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            int noteIndex = Mathf.FloorToInt(t / beat) % melody.Length;
            float noteT = (t - Mathf.FloorToInt(t / beat) * beat) / beat;
            float env = Mathf.Exp(-noteT * 2.5f) * 0.35f;
            float val = Mathf.Sin(2f * Mathf.PI * melody[noteIndex] * t) * env;
            // Bass pedal tone
            val += Mathf.Sin(2f * Mathf.PI * 98f * t) * 0.12f;
            data[i] = val * volume;
        }
        AudioClip clip = AudioClip.Create("StageSelectBGM", samples, 1, sampleRate, true);
        clip.SetData(data, 0);
        return clip;
    }
}
