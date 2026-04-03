using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    public GameObject settingsPanel;


    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&& !settingsPanel.activeInHierarchy && !QuitMenu.instance.menuPanel.activeInHierarchy)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    // ⏸ Pause game
    void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenSettings()
    {
        
            settingsPanel.SetActive(!settingsPanel.activeSelf);
            
            pausePanel.SetActive(false); 
        
    }
}