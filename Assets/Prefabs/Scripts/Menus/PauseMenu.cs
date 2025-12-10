using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool GameIsPaused = false;

    void Start()
    {
        
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        { 
			Debug.Log("P pressed.");
            Pause();
}
    }

    public void Resume()
    {
        
        GameIsPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
       Debug.Log("resume");
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
Debug.Log("pause");
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit(); 
        Debug.Log("Game exited");
    }
}