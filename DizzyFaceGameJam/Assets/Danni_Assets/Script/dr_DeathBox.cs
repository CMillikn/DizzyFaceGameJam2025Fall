using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class dr_DeathBox : MonoBehaviour
{
    [Header("Layer Settings")]
    public string ballLayerName = "Ball";   // the layer name used by the ball

    [Header("SFX (optional)")]
    public AudioClip fallClip;              // play once when ball hits deathbox
    public UnityEngine.Audio.AudioMixerGroup sfxOutput; // route to SFX mixer
    public float sfxCooldown = 0.2f;        // avoid spam if multiple colliders
    public bool debugLogs = false;

    private int ballLayer;
    private AudioSource _src;
    private float _lastTime;

    void Awake()
    {
        // ensure collider is trigger
        var box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;

        // optional but robust: kinematic rb for stable trigger callbacks
        var rb = GetComponent<Rigidbody2D>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.simulated = true;
        rb.gravityScale = 0f;

        // audio source for sfx (if a clip is assigned)
        _src = gameObject.AddComponent<AudioSource>();
        _src.playOnAwake = false;
        _src.loop = false;
        _src.spatialBlend = 0f;
        _src.outputAudioMixerGroup = sfxOutput;

        ballLayer = LayerMask.NameToLayer(ballLayerName);
        if (ballLayer == -1) Debug.LogWarning($"dr_DeathBox: Layer '{ballLayerName}' not found.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != ballLayer)
        {
            if (debugLogs) Debug.Log($"DeathBox ignore: {other.name} layer={other.gameObject.layer}");
            return;
        }

        if (debugLogs) Debug.Log($"DeathBox: ball exited -> {other.name}");

        // play fall sfx (optional)
        if (fallClip && Time.time - _lastTime >= sfxCooldown)
        {
            _lastTime = Time.time;
            _src.pitch = Random.Range(0.98f, 1.02f);
            _src.PlayOneShot(fallClip, 1f);
        }

        // show game over UI
        var ui = FindObjectOfType<dr_UIManager>();
        if (ui) ui.ShowGameOver();
    }
}
