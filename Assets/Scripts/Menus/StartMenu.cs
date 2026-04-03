using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    
    public GameObject settingsPanel;
    public GameObject levelPanel;
    public GameObject mainMenuPanel;

    
    public void StartGame()
    {
        SceneManager.LoadScene("TheBunkerScene");
        Debug.Log("StartGame");
    }
    public void OpenLevels()
    {
        levelPanel.SetActive(!levelPanel.activeSelf);
        mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);

    }

    public void QuitGame()
    {
        Application.Quit();

      
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}
