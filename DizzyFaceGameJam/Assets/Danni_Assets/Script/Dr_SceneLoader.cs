using UnityEngine;
using UnityEngine.SceneManagement;

public class Dr_SceneLoader : MonoBehaviour
{
    public string gameSceneName = "Game";
    public string titleSceneName = "Title";

    public void StartGame() { SceneManager.LoadScene(gameSceneName); }
    public void Retry() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void ExitToTitle() { SceneManager.LoadScene(titleSceneName); }
}
