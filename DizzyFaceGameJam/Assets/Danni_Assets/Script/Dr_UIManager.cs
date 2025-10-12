using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class dr_UIManager : MonoBehaviour
{
    public static bool skipTitleOnce = false;

    [Header("Panels")]
    public GameObject titlePanel;
    public GameObject hudPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;
    public GameObject howToPanel;    // How To Play page
    public GameObject winPanel;     // Win screen



    [Header("HUD")]
    public TMP_Text txtScore;
    public TMP_Text txtTimer;

    [Header("GameOver Texts")]
    public TMP_Text txtGOTime;
    public TMP_Text txtGOBestTime;
    public TMP_Text txtWinTime;
    public TMP_Text txtWinBest;

    [Header("Audio Mixer")]
    public AudioMixer mixer;
    public Slider sliderMaster;
    public Slider sliderMusic;
    public Slider sliderSFX;
    public string paramMaster = "MasterVol";
    public string paramMusic = "MusicVol";
    public string paramSFX = "SFXVol";
    [Range(-80f, 0f)] public float minDb = -40f;

    [Header("UI Events")]
    public UnityEvent OnStartPressed;
    public UnityEvent OnRetryPressed;
    public UnityEvent OnExitToTitlePressed;
    public UnityEvent OnPauseOpened;
    public UnityEvent OnPauseClosed;

    private bool paused = false;

    // Survival Time
    const string PP_BestTime = "BestTime";
    private float runTime = 0f;
    private float bestTime = 0f;
    private bool timerRunning = false;

    void Awake()
    {
        bestTime = PlayerPrefs.GetFloat(PP_BestTime, 0f);

        if (sliderMaster) sliderMaster.onValueChanged.AddListener(SetMasterVolume);
        if (sliderMusic) sliderMusic.onValueChanged.AddListener(SetMusicVolume);
        if (sliderSFX) sliderSFX.onValueChanged.AddListener(SetSFXVolume);

        if (skipTitleOnce)
        {
            skipTitleOnce = false;
            HideAll();
            SetPanel(hudPanel, true);
            Time.timeScale = 1f;
            paused = false;
            BeginRunTimer();
        }
        else
        {
            ShowTitle();
            Time.timeScale = 0f;
            timerRunning = false;
            SetTimer(0f);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Btn_PauseToggle();

        if (timerRunning)
        {
            runTime += Time.deltaTime;
            SetTimer(runTime);
        }
    }

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

    void HideAll()
    {
        SetPanel(titlePanel, false);
        SetPanel(hudPanel, false);
        SetPanel(pausePanel, false);
        SetPanel(gameOverPanel, false);
        SetPanel(settingsPanel, false);
    }

    public void ShowTitle()
    {
        HideAll();
        SetPanel(titlePanel, true);
        Time.timeScale = 1f;
        paused = false;
    }

    public void ShowHUD()
    {
        HideAll();
        SetPanel(hudPanel, true);
        Time.timeScale = 1f;
        paused = false;
        timerRunning = true;
    }

    public void ShowPause()
    {
        SetPanel(pausePanel, true);
        Time.timeScale = 0f;
        paused = true;
        OnPauseOpened?.Invoke();
        timerRunning = false;
    }

    public void HidePause()
    {
        SetPanel(pausePanel, false);
        Time.timeScale = 1f;
        paused = false;
        OnPauseClosed?.Invoke();
        timerRunning = true;
    }

    public void ShowGameOver()
    {
        HideAll();
        SetPanel(gameOverPanel, true);
        Time.timeScale = 0f;

        timerRunning = false;
        if (txtGOTime) txtGOTime.text = $"Time: {runTime:0.0}s";

        if (runTime > bestTime)
        {
            bestTime = runTime;
            PlayerPrefs.SetFloat(PP_BestTime, bestTime);
            PlayerPrefs.Save();
        }
        if (txtGOBestTime) txtGOBestTime.text = $"Best: {bestTime:0.0}s";
    }

    public void ShowWin()
    {
        HideAll();
        SetPanel(winPanel, true);
        Time.timeScale = 0f;

        timerRunning = false;

        // current run clear time
        if (txtWinTime) txtWinTime.text = $"Time: {runTime:0.0}s";

        // best = minimum time to goal
        if (bestTime <= 0f || runTime < bestTime)
        {
            bestTime = runTime;
            PlayerPrefs.SetFloat(PP_BestTime, bestTime);
            PlayerPrefs.Save();
        }

        if (txtWinBest) txtWinBest.text = $"Best: {bestTime:0.0}s";
    }



    public void ShowSettings(bool show)
    {
        SetPanel(settingsPanel, show);
    }

    public void Btn_StartGame()
    {
        if (OnStartPressed != null && OnStartPressed.GetPersistentEventCount() > 0)
            OnStartPressed.Invoke();
        else
            ShowHUD();

        BeginRunTimer();
    }

    public void Btn_PauseToggle()
    {
        if (paused) HidePause();
        else ShowPause();
    }

    public void Btn_OpenSettings() { ShowSettings(true); }
    public void Btn_CloseSettings() { ShowSettings(false); }

    public void Btn_Retry()
    {
        Time.timeScale = 1f;

        if (OnRetryPressed != null && OnRetryPressed.GetPersistentEventCount() > 0)
            OnRetryPressed.Invoke();
        else
        {
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
            Time.timeScale = 1f;
            dr_UIManager.skipTitleOnce = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void SetScore(int v)
    {
        if (txtScore) txtScore.text = $"Score: {v}";
    }

    public void SetTimer(float t)
    {
        if (txtTimer) txtTimer.text = $"Time: {t:0.0}s";
    }

    public void SetMasterVolume(float v) { SetDb(paramMaster, v); }
    public void SetMusicVolume(float v) { SetDb(paramMusic, v); }
    public void SetSFXVolume(float v) { SetDb(paramSFX, v); }

    void SetDb(string param, float v)
    {
        if (!mixer || string.IsNullOrEmpty(param)) return;
        float db = Mathf.Lerp(minDb, 0f, Mathf.Clamp01(v));
        mixer.SetFloat(param, db);
    }

    float DbTo01(float db)
    {
        return Mathf.InverseLerp(minDb, 0f, db);
    }

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

    void BeginRunTimer()
    {
        runTime = 0f;
        timerRunning = true;
        SetTimer(runTime);
    }

    public void Btn_OpenHowTo()
    {
        SetPanel(howToPanel, true);
        SetPanel(titlePanel, false);
        // keep timescale frozen at title; no need to change Time.timeScale
    }

    public void Btn_CloseHowTo()
    {
        SetPanel(howToPanel, false);
        SetPanel(titlePanel, true);
        // back to title; still frozen
    }

}
