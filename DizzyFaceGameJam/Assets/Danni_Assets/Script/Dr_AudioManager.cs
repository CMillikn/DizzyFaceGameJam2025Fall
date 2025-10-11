using UnityEngine;
using UnityEngine.Audio;

public class dr_AudioManager : MonoBehaviour
{

    public static dr_AudioManager I;

    [Header("Mixer")]
    public AudioMixer mixer;                 // MasterMixer
    public string paramMaster = "MasterVol";
    public string paramMusic  = "MusicVol";
    public string paramSFX    = "SFXVol";

    [Header("Sources")]
    public AudioSource musicSource;          // Loop
    public AudioSource sfxSource;            // OneShot
    public AudioSource ambienceSource;       

    [Header("Clips")]
    public AudioClip bgmLoop;
    public AudioClip uiClick;
    public AudioClip sfxGameOver;
    public AudioClip sfxGoal;

    void Awake(){
        if (I == null){ I = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
    }

    void Start(){
        if (bgmLoop){
            musicSource.clip = bgmLoop;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip, float vol = 1f){
        if (clip && sfxSource) sfxSource.PlayOneShot(clip, vol);
    }
    public void PlayUIClick(){ PlaySFX(uiClick, 1f); }

    public void SetMaster(float v){ SetDb(paramMaster, v); }
    public void SetMusic (float v){ SetDb(paramMusic,  v); }
    public void SetSFX   (float v){ SetDb(paramSFX,    v); }

    void SetDb(string param, float v){
        if (!mixer) return;
        float db = Mathf.Lerp(-40f, 0f, Mathf.Clamp01(v)); // 0..1  -40..0 dB
        mixer.SetFloat(param, db);
    }
}
