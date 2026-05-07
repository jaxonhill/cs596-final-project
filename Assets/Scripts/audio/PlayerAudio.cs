using UnityEngine;
 
/// <summary>
/// Player (Human Knight) — Audio Controller
/// Attach to the Player's root GameObject.
///
/// SETUP IN INSPECTOR:
///   Add 1 AudioSource component and assign it below.
///   - audioSource : spatialBlend = 0, no loop
///
/// PARTNER HOOKS:
///   Animation event (walk/run) → OnFootstep()
///   State enter (jump)         → PlayJump()
///   State enter (roll)         → PlayRoll()
///   State enter (attack)       → PlaySwordSwing()
///   Damageable.Damaged         → PlayTakeDamage()
///   Damageable.Died            → PlayDeath()
/// </summary>
public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Locomotion")]
    [SerializeField] private AudioClip[] runStepClips;
    [SerializeField] private AudioClip[] jumpClips;
    [SerializeField] private AudioClip[] rollClips;

    [Header("Combat")]
    [SerializeField] private AudioClip[] swordSwingClips;
    [SerializeField] private AudioClip[] takeDamageClips;
    [SerializeField] private AudioClip   deathClip;

    private bool _isDead          = false;
    private int  _takeDamageIndex = 0;

    // ── Called by animation event on each footfall ──────────────────
    public void OnFootstep()
    {
        if (_isDead) return;
        PlayRandom(audioSource, runStepClips);
    }

    public void PlayJump()
    {
        if (_isDead) return;
        PlayRandom(audioSource, jumpClips);
    }

    // ── Call from movement controller when player rolls ──────────────
    public void PlayRoll()
    {
        if (_isDead) return;
        PlayRandom(audioSource, rollClips);
    }

    // ── Called by animation event at swing release frame ────────────
    public void PlaySwordSwing()
    {
        if (_isDead) return;
        PlayRandom(audioSource, swordSwingClips);
    }

    // ── Call from TakeDamage() ───────────────────────────────────────
    public void PlayTakeDamage()
    {
        if (_isDead || takeDamageClips == null || takeDamageClips.Length == 0) return;
        if (audioSource == null) return;
        audioSource.PlayOneShot(takeDamageClips[_takeDamageIndex % takeDamageClips.Length]);
        _takeDamageIndex++;
    }

    // ── Call when health <= 0 ────────────────────────────────────────
    public void PlayDeath()
    {
        if (_isDead) return;
        _isDead = true;
        if (audioSource != null)
        {
            audioSource.Stop();
            if (deathClip != null) audioSource.PlayOneShot(deathClip);
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────
    private static AudioClip PickRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }

    private static void PlayRandom(AudioSource source, AudioClip[] clips)
    {
        if (source == null) return;
        AudioClip clip = PickRandom(clips);
        if (clip != null) source.PlayOneShot(clip);
    }
}
