using UnityEngine;

/// <summary>
/// SoundManager — Global Audio Manager
/// Singleton prefab that persists across all scenes.
/// Handles master/SFX volume and a global 2D one-shot pool.
/// No music — add that later if needed.
///
/// SETUP:
///   1. Create an empty GameObject, name it "SoundManager"
///   2. Attach this script
///   3. Add 1 AudioSource component → assign to sfxSource slot
///   4. Drag into Prefabs folder
///   5. Place in your first/boot scene — survives all scene loads
///
/// USAGE FROM ANY SCRIPT:
///   SoundManager.Instance.PlaySFX(clip);
///   SoundManager.Instance.SetMasterVolume(0.8f);
///   SoundManager.Instance.SetSFXVolume(0.5f);
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Default Volumes")]
    [Range(0f, 1f)] [SerializeField] private float defaultMasterVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float defaultSFXVolume    = 1f;

    private const string PREF_MASTER = "vol_master";
    private const string PREF_SFX    = "vol_sfx";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadVolumeSettings();
    }

    // ── Volume control ───────────────────────────────────────────────

    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(PREF_MASTER, volume);
        AudioListener.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(PREF_SFX, volume);
        if (sfxSource != null) sfxSource.volume = volume;
    }

    // ── Global 2D one-shot SFX ───────────────────────────────────────

    /// <summary>Play a 2D one-shot sound from anywhere in the project.</summary>
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    /// <summary>Play a random clip from an array.</summary>
    public void PlaySFXRandom(AudioClip[] clips, float volume = 1f)
    {
        if (clips == null || clips.Length == 0) return;
        PlaySFX(clips[Random.Range(0, clips.Length)], volume);
    }

    // ── Helpers ──────────────────────────────────────────────────────

    private void LoadVolumeSettings()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(PREF_MASTER, defaultMasterVolume));
        SetSFXVolume(PlayerPrefs.GetFloat(PREF_SFX, defaultSFXVolume));
    }
}