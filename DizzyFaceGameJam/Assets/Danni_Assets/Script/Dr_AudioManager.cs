using UnityEngine;
using UnityEngine.Audio;

public class dr_AudioManager : MonoBehaviour
{
    // Singleton instance so we can easily access this AudioManager from anywhere
    public static dr_AudioManager I;

    [Header("Mixer")]
    public AudioMixer mixer;                  // The main audio mixer that controls all volume levels
    public string paramMaster = "MasterVol";  // Parameter name for master volume
    public string paramMusic  = "MusicVol";   // Parameter name for music volume
    public string paramSFX    = "SFXVol";     // Parameter name for sound effect volume

    [Header("Sources")]
    public AudioSource musicSource;           // Used for background music (loop)
    public AudioSource sfxSource;             // Used for one-shot sound effects
    public AudioSource ambienceSource;        // (Optional) Used for ambient sounds

    [Header("Clips")]
    public AudioClip bgmLoop;                 // Background music clip
    public AudioClip uiClick;                 // UI click sound
    public AudioClip sfxGameOver;             // Game over sound
    public AudioClip sfxGoal;                 // Goal or win sound

    void Awake(){
        // Make sure only one AudioManager exists across all scenes
        if (I == null){
            I = this; DontDestroyOnLoad(gameObject); // Keep this object when changing scenes
        }
        else {
            Destroy(gameObject);                    // If another exists, destroy this one
            return; 
        }
    }

    void Start(){
        // Play background music when the game starts
        if (bgmLoop){
            musicSource.clip = bgmLoop;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // Play any sound effect with adjustable volume (default = 1)
    public void PlaySFX(AudioClip clip, float vol = 1f){
        if (clip && sfxSource) sfxSource.PlayOneShot(clip, vol);
    }
    // Quick method for UI click sound
    public void PlayUIClick(){ PlaySFX(uiClick, 1f); }

    // Control volume through mixer sliders
    public void SetMaster(float v){ SetDb(paramMaster, v); }
    public void SetMusic (float v){ SetDb(paramMusic,  v); }
    public void SetSFX   (float v){ SetDb(paramSFX,    v); }

    // Convert 0–1 volume slider to decibels (-40dB to 0dB)
    void SetDb(string param, float v){
        if (!mixer) return;
        float db = Mathf.Lerp(-40f, 0f, Mathf.Clamp01(v));
        mixer.SetFloat(param, db);
    }
}
