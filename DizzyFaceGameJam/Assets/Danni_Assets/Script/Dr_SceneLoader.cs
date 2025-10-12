using UnityEngine;
using UnityEngine.SceneManagement;

public class Dr_SceneLoader : MonoBehaviour
{
    // The name of the main gameplay scene
    public string gameSceneName = "Game";
    // The name of the title or main menu scene
    public string titleSceneName = "Title";

    // Called when player starts the game
    public void StartGame() { SceneManager.LoadScene(gameSceneName); }
    // Reloads the current scene (used for Retry button)
    public void Retry() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    // Returns to the title screen
    public void ExitToTitle() { SceneManager.LoadScene(titleSceneName); }
}
