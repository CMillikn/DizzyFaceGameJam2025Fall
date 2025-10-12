using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement; // for scene reload on Restart/Exit

public class dr_UIManager : MonoBehaviour
{
    // skip title once after Restart (persists across scene reload within play session)
    public static bool skipTitleOnce = false;

    [Header("Panels")]
    public GameObject titlePanel;        // Title screen UI
    public GameObject hudPanel;          // In-game HUD
    public GameObject pausePanel;        // Pause menu
    public GameObject gameOverPanel;     // Game over screen
    public GameObject settingsPanel;     // Settings menu
    public GameObject creditsPanel;      // Credits page

    [Header("HUD")]
    public TMP_Text txtScore;            // Score display text
    public TMP_Text txtTimer;            // Timer display text

    [Header("Audio Mixer")]
    public AudioMixer mixer;             // Main mixer that controls all volumes
    public Slider sliderMaster;          // Master volume slider
    public Slider sliderMusic;           // Music volume slider
    public Slider sliderSFX;             // SFX volume slider
    public string paramMaster = "MasterVol";    // Mixer parameter name for master volume
    public string paramMusic = "MusicVol";      // Mixer parameter name for music
    public string paramSFX = "SFXVol";          // Mixer parameter name for SFX
    [Range(-80f, 0f)] public float minDb = -40f; // Minimum decibel level

    [Header("UI Events")]
    public UnityEvent OnStartPressed;         // Triggered when Start button is pressed
    public UnityEvent OnRetryPressed;         // Triggered when Retry button is pressed
    public UnityEvent OnExitToTitlePressed;   // Triggered when Exit button is pressed
    public UnityEvent OnPauseOpened;          // Triggered when Pause menu opens
    public UnityEvent OnPauseClosed;          // Triggered when Pause menu closes

    bool paused = false;             // Track whether the game is paused

    void Awake()
    {
        // Add listeners to volume sliders
        if (sliderMaster) sliderMaster.onValueChanged.AddListener(SetMasterVolume);
        if (sliderMusic) sliderMusic.onValueChanged.AddListener(SetMusicVolume);
        if (sliderSFX) sliderSFX.onValueChanged.AddListener(SetSFXVolume);

        // If coming from Restart, skip title once and start directly in HUD
        if (skipTitleOnce)
        {
            skipTitleOnce = false;
            HideAll();
            SetPanel(hudPanel, true);
            Time.timeScale = 1f;
            paused = false;
        }
        else
        {
            // Show title screen at startup and freeze gameplay until Start
            ShowTitle();
            Time.timeScale = 0f;
        }
    }

    // Show or hide a panel smoothly using CanvasGroup
    void SetPanel(GameObject go, bool show)
    {
        if (!go) return;
        var cg = go.GetComponent<CanvasGroup>();
        if (!cg) cg = go.AddComponent<CanvasGroup>();
        cg.alpha = show ? 1f : 0f;
        cg.interactable = show;
        cg.blocksRaycasts = show;
        go.SetActive(true);
    }

    // Hide all major panels
    void HideAll()
    {
        SetPanel(titlePanel, false);
        SetPanel(hudPanel, false);
        SetPanel(pausePanel, false);
        SetPanel(gameOverPanel, false);
        SetPanel(settingsPanel, false);
    }

    // --- Panel control methods ---
    public void ShowTitle() { HideAll(); SetPanel(titlePanel, true); Time.timeScale = 1f; paused = false; }
    public void ShowHUD() { HideAll(); SetPanel(hudPanel, true); Time.timeScale = 1f; paused = false; }
    public void ShowPause() { SetPanel(pausePanel, true); Time.timeScale = 0f; paused = true; OnPauseOpened?.Invoke(); }
    public void HidePause() { SetPanel(pausePanel, false); Time.timeScale = 1f; paused = false; OnPauseClosed?.Invoke(); }
    public void ShowGameOver() { HideAll(); SetPanel(gameOverPanel, true); Time.timeScale = 0f; }
    public void ShowSettings(bool show) { SetPanel(settingsPanel, show); }

    // --- Button methods ---
    public void Btn_StartGame()
    {
        if (OnStartPressed != null && OnStartPressed.GetPersistentEventCount() > 0)
            OnStartPressed.Invoke();
        else
            ShowHUD();
    }

    public void Btn_PauseToggle() { if (paused) HidePause(); else ShowPause(); }
    public void Btn_OpenSettings() { ShowSettings(true); }
    public void Btn_CloseSettings() { ShowSettings(false); }

    // Restart current level from Pause (or anywhere): unpause first, then either event or hard reload
    public void Btn_Retry()
    {
        Time.timeScale = 1f; // ensure unpaused before restart

        if (OnRetryPressed != null && OnRetryPressed.GetPersistentEventCount() > 0)
            OnRetryPressed.Invoke();
        else
        {
            // mark to skip title on the next scene load, then reload this scene
            dr_UIManager.skipTitleOnce = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void Btn_ExitToTitle()
    {
        if (OnExitToTitlePressed != null && OnExitToTitlePressed.GetPersistentEventCount() > 0)
        {
            OnExitToTitlePressed.Invoke();
        }
        else
        {
            // NEW: hard-reload the scene and DO NOT skip title
            Time.timeScale = 1f;        // make sure we don't carry a paused state into load
            dr_UIManager.skipTitleOnce = false; // ensure next Awake shows Title
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // <- fresh scene = fresh run
        }

        // NOTE: no manual ShowTitle()/Time.timeScale=0 here,
        // the freshly loaded scene's Awake will show Title and freeze it.
    }

    // --- HUD updates ---
    public void SetScore(int v) { if (txtScore) txtScore.text = $"Score: {v}"; }
    public void SetTimer(float t) { if (txtTimer) txtTimer.text = $"Time: {t:0.0}s"; }

    // --- Volume controls ---
    public void SetMasterVolume(float v) { SetDb(paramMaster, v); }
    public void SetMusicVolume(float v) { SetDb(paramMusic, v); }
    public void SetSFXVolume(float v) { SetDb(paramSFX, v); }

    // Convert 0–1 slider value to decibel and apply to mixer
    void SetDb(string param, float v)
    {
        if (!mixer || string.IsNullOrEmpty(param)) return;
        float db = Mathf.Lerp(minDb, 0f, Mathf.Clamp01(v));
        mixer.SetFloat(param, db);
    }

    // Convert decibel value back to 0–1 for slider refresh
    float DbTo01(float db)
    {
        return Mathf.InverseLerp(minDb, 0f, db);
    }

    // Refresh all volume sliders to match current mixer values
    public void RefreshVolumeSliders()
    {
        if (!mixer) return;
        float db;
        if (sliderMaster && mixer.GetFloat(paramMaster, out db))
            sliderMaster.SetValueWithoutNotify(DbTo01(db));
        if (sliderMusic && mixer.GetFloat(paramMusic, out db))
            sliderMusic.SetValueWithoutNotify(DbTo01(db));
        if (sliderSFX && mixer.GetFloat(paramSFX, out db))
            sliderSFX.SetValueWithoutNotify(DbTo01(db));
    }

    // --- Credits panel controls ---
    public void Btn_OpenCredits()
    {
        SetPanel(creditsPanel, true);
        SetPanel(titlePanel, false);
    }

    public void Btn_CloseCredits()
    {
        SetPanel(creditsPanel, false);
        SetPanel(titlePanel, true);
    }

    void Update()
    {
        // Allow player to toggle Pause with 'P' key
        if (Input.GetKeyDown(KeyCode.P))
        {
            Btn_PauseToggle();
        }
    }

}
