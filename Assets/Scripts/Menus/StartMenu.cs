using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    
    public GameObject settingsPanel;
    public GameObject settingsPanel1;

    
    public void StartGame()
    {
        SceneManager.LoadScene("TheBunkerScene");
        Debug.Log("StartGame");
    }
    public void OpenLevels()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        settingsPanel1.SetActive(!settingsPanel1.activeSelf);

    }

    public void QuitGame()
    {
        Application.Quit();

      
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
