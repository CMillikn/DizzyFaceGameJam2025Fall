using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(Collider2D))]
public class dr_BallSFX : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip hitA; // First hit sound
    public AudioClip hitB; // Second hit sound (alternates)

    [Header("Audio Routing")]
    public AudioMixerGroup sfxOutput; // Assign your SFX mixer group here

    [Header("Filter Settings")]
    public LayerMask playerLayers;    // Layers considered as “player” (e.g., PlayerHitbox, Bumpers)
    public float cooldown = 0.05f;    // Minimum delay between consecutive hit sounds
    public bool ignorePlayerFilter = false; // When true, plays hit for any collision (useful for testing)
    public bool debugLogs = false;    // Prints debug messages to the console if enabled

    private AudioSource _src;
    private bool _toggle;             // Toggles between hitA and hitB
    private float _lastTime;

    void Awake()
    {
        // Create and configure the AudioSource
        _src = gameObject.AddComponent<AudioSource>();
        _src.playOnAwake = false;
        _src.loop = false;
        _src.spatialBlend = 0f;
        _src.outputAudioMixerGroup = sfxOutput;
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (debugLogs)
            Debug.Log($"Ball COLLISION with: {c.collider.name} layer={c.collider.gameObject.layer}");

        // If filter is off, or collider belongs to player layer, play sound
        if (ignorePlayerFilter || IsPlayer(c.collider))
            PlayHit("collision");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (debugLogs)
            Debug.Log($"Ball TRIGGER with: {other.name} layer={other.gameObject.layer}");

        if (ignorePlayerFilter || IsPlayer(other))
            PlayHit("trigger");
    }

    // Checks if the collider belongs to any of the selected player layers
    bool IsPlayer(Collider2D col)
    {
        bool layerHit = (playerLayers.value & (1 << col.gameObject.layer)) != 0;
        if (debugLogs)
            Debug.Log($"IsPlayer? {col.name} -> {layerHit}");
        return layerHit;
    }

    // Plays alternating hit sounds with cooldown
    void PlayHit(string src)
    {
        if (Time.time - _lastTime < cooldown) return;
        _lastTime = Time.time;

        AudioClip clip = null;

        // Alternate between hitA / hitB each time
        if (hitA && hitB)
        {
            clip = _toggle ? hitA : hitB;
            _toggle = !_toggle;
        }
        else
            clip = hitA ? hitA : hitB;

        if (debugLogs)
            Debug.Log($"PlayHit via {src}. clip={(clip ? clip.name : "NULL")}");

        if (clip)
            _src.PlayOneShot(clip, 1f);
    }
}
