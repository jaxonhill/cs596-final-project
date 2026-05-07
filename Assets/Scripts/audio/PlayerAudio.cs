using UnityEngine;
 
/// <summary>
/// Player (Human Knight) — Audio Controller
/// Attach to the Player's root GameObject.
///
/// SETUP IN INSPECTOR:
///   Add 2 AudioSource components and assign them below.
///   - audioSourceFootstep : spatialBlend = 0, no loop
///   - audioSourceCombat   : spatialBlend = 0, no loop
///
/// PARTNER HOOKS:
///   Animation event (walk/run) → OnFootstep()
///   Movement controller        → SetRunning(bool)
///   Animation event (swing)    → OnSwingRelease()
///   DealDamage()               → PlaySwordHit()
///   TakeDamage()               → PlayTakeDamage()
///   health <= 0                → PlayDeath()
/// </summary>
public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSourceFootstep;
    [SerializeField] private AudioSource audioSourceCombat;
 
    [Header("Footsteps — Walk (per surface)")]
    [SerializeField] private AudioClip[] walkStepsStone;
    [SerializeField] private AudioClip[] walkStepsWood;
    [SerializeField] private AudioClip[] walkStepsDirt;
    [SerializeField] private AudioClip[] walkStepsDefault;
 
    [Header("Footsteps — Run (per surface)")]
    [SerializeField] private AudioClip[] runStepsStone;
    [SerializeField] private AudioClip[] runStepsWood;
    [SerializeField] private AudioClip[] runStepsDirt;
    [SerializeField] private AudioClip[] runStepsDefault;
 
    [Header("Movement")]
    [SerializeField] private AudioClip[] rollClips;
 
    [Header("Combat")]
    [SerializeField] private AudioClip[] swordSwingMissClips;
    [SerializeField] private AudioClip[] swordSwingHitClips;
    [SerializeField] private AudioClip[] takeDamageClips;
    [SerializeField] private AudioClip   deathClip;
 
    [Header("Surface Detection")]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float     raycastDistance = 1.5f;
 
    private bool _isDead          = false;
    private bool _isRunning       = false;
    private int  _takeDamageIndex = 0;
 
    private const string TAG_STONE = "Stone";
    private const string TAG_WOOD  = "Wood";
    private const string TAG_DIRT  = "Dirt";
 
    // ── Called by animation event on each footfall ──────────────────
    public void OnFootstep()
    {
        if (_isDead) return;
        string    surface = DetectSurface();
        AudioClip clip    = _isRunning ? PickRunClip(surface) : PickWalkClip(surface);
        if (clip == null) return;
        audioSourceFootstep.PlayOneShot(clip);
    }
 
    // ── Call from movement controller when walk ↔ run ───────────────
    public void SetRunning(bool running) => _isRunning = running;
 
    // ── Call from movement controller when player rolls ──────────────
    public void PlayRoll()
    {
        if (_isDead) return;
        PlayRandom(audioSourceFootstep, rollClips);
    }
 
    // ── Called by animation event at swing release frame ────────────
    public void OnSwingRelease()
    {
        if (_isDead) return;
        PlayRandom(audioSourceCombat, swordSwingMissClips);
    }
 
    // ── Call from DealDamage() on confirmed hit ──────────────────────
    public void PlaySwordHit()
    {
        if (_isDead) return;
        audioSourceCombat.Stop();
        PlayRandom(audioSourceCombat, swordSwingHitClips);
    }
 
    // ── Call from TakeDamage() ───────────────────────────────────────
    public void PlayTakeDamage()
    {
        if (_isDead || takeDamageClips == null || takeDamageClips.Length == 0) return;
        audioSourceCombat.PlayOneShot(takeDamageClips[_takeDamageIndex % takeDamageClips.Length]);
        _takeDamageIndex++;
    }
 
    // ── Call when health <= 0 ────────────────────────────────────────
    public void PlayDeath()
    {
        if (_isDead) return;
        _isDead = true;
        audioSourceFootstep.Stop();
        audioSourceCombat.Stop();
        if (deathClip != null) audioSourceCombat.PlayOneShot(deathClip);
    }
 
    // ── Helpers ──────────────────────────────────────────────────────
    private string DetectSurface()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, groundLayerMask))
            return hit.collider.tag;
        return string.Empty;
    }
 
    private AudioClip PickWalkClip(string tag) => tag switch
    {
        TAG_STONE => PickRandom(walkStepsStone),
        TAG_WOOD  => PickRandom(walkStepsWood),
        TAG_DIRT  => PickRandom(walkStepsDirt),
        _         => PickRandom(walkStepsDefault)
    };
 
    private AudioClip PickRunClip(string tag) => tag switch
    {
        TAG_STONE => PickRandom(runStepsStone),
        TAG_WOOD  => PickRandom(runStepsWood),
        TAG_DIRT  => PickRandom(runStepsDirt),
        _         => PickRandom(runStepsDefault)
    };
 
    private static AudioClip PickRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }
 
    private static void PlayRandom(AudioSource source, AudioClip[] clips)
    {
        AudioClip clip = PickRandom(clips);
        if (clip != null) source.PlayOneShot(clip);
    }
}