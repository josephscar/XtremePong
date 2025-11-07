using UnityEngine;

public class SFX : MonoBehaviour
{
    public static SFX I;

    [Header("Clips")]
    public AudioClip[] hitClips;
    public AudioClip[] scoreClips;
    public AudioClip[] serveClips;     // ball launches from center
    public AudioClip[] gameEndClips;   // game-over sound (not wired yet)

    [Header("Tuning")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 0.2f)] public float hitPitchJitter = 0.08f;

    AudioSource src;

    void Awake()
    {
        if (I) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.spatialBlend = 0f;  // 2D sound
    }

    public void PlayHit(float intensity01 = 1f)
    {
        float vol = Mathf.Clamp01(0.25f + 0.75f * intensity01);
        float pitch = 1f + Random.Range(-hitPitchJitter, hitPitchJitter);
        PlayRandom(hitClips, vol, pitch);
    }

    public void PlayScore()
    {
        PlayRandom(scoreClips, 1f, 1f + Random.Range(-0.02f, 0.02f));
    }

    void PlayRandom(AudioClip[] pool, float volume, float pitch)
    {
        if (pool == null || pool.Length == 0) return;
        var clip = pool[Random.Range(0, pool.Length)];
        src.pitch = pitch;
        src.PlayOneShot(clip, volume * masterVolume);
    }

    public void PlayServe()
    {
        PlayRandom(serveClips, 1f, 1f + Random.Range(-0.03f, 0.03f));
    }

    public void PlayGameEnd()
    {
        // NOTE: You asked not to wire this yet.
        PlayRandom(gameEndClips, 1f, 1f);
    }
}
