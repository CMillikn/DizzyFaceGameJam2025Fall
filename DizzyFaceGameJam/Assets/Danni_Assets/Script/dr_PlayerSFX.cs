using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(Collider2D))]
public class dr_PlayerSFX : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip jumpClip;        // Jump sound
    public AudioClip landClip;        // Landing sound

    [Header("Audio Routing (Mixer)")]
    public AudioMixerGroup sfxOutput; // Assign the SFX mixer group here

    [Header("Audio Settings")]
    public float minLandSpeed = 2.0f; // Minimum Y velocity to trigger landing sound
    public float cooldown = 0.05f;    // Prevents overlapping SFX spam

    private AudioSource _src;
    private Conrad_PlayerScript _player;
    private float _lastTime;

    void Awake()
    {
        // Create and configure the AudioSource
        _player = GetComponent<Conrad_PlayerScript>();

        _src = gameObject.AddComponent<AudioSource>();
        _src.playOnAwake = false;
        _src.loop = false;
        _src.spatialBlend = 0f;
        _src.outputAudioMixerGroup = sfxOutput;
    }

    void Update()
    {
        // Play jump sound when grounded player presses space
        if (Input.GetKeyDown(KeyCode.Space) && _player != null && _player.isGrounded)
            PlayOneShot(jumpClip);
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        // Detect strong enough landing
        float speed = Mathf.Abs(c.relativeVelocity.y);
        if (speed >= minLandSpeed)
            PlayOneShot(landClip);
    }

    // Safely plays one-shot audio clip with cooldown and random pitch
    private void PlayOneShot(AudioClip clip, float vol = 1f)
    {
        if (!clip) return;
        if (Time.time - _lastTime < cooldown) return;
        _lastTime = Time.time;

        _src.pitch = Random.Range(0.97f, 1.03f); // slight pitch variation
        _src.PlayOneShot(clip, Mathf.Clamp01(vol));
    }
}
