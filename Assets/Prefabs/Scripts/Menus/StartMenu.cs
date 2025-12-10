using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    
    public void StartGame()
    {
        // Load your game scene
        SceneManager.LoadScene("TheBunkerScene");
        Debug.Log("StartGame");
    }

    public void OpenLevels()
    {
        // Load your level selection scene
        SceneManager.LoadScene("LevelSelect");
    }

    public void OpenSettings()
    {
        // Load your settings scene
        SceneManager.LoadScene("Settings");
    }

    public void QuitGame()
    {
        // Quit application
        Application.Quit();

        // If running in editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
