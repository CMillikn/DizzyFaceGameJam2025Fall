using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class dr_Goal : MonoBehaviour
{
    [Header("Layer Settings")]
    public string ballLayerName = "Ball";  
    public AudioClip winClip;               
    public UnityEngine.Audio.AudioMixerGroup sfxOutput;

    private int ballLayer;
    private AudioSource src;
    private bool triggered = false;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true; 

        ballLayer = LayerMask.NameToLayer(ballLayerName);
        if (ballLayer == -1)
            Debug.LogWarning($"dr_Goal: layer '{ballLayerName}' not found.");

        
        src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.spatialBlend = 0f;
        src.outputAudioMixerGroup = sfxOutput;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return; 
        if (other.gameObject.layer != ballLayer) return;

        triggered = true;
        if (winClip) src.PlayOneShot(winClip, 1f);

        
        var ui = FindObjectOfType<dr_UIManager>();
        if (ui != null)
        {
            ui.ShowWin();
            Debug.Log(" Win! Ball reached goal.");
        }
    }
}
