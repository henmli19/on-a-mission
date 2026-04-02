using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitMenu : MonoBehaviour
{
    public static QuitMenu instance;
    public GameObject menuPanel; // assign your menu UI panel in Inspector


    void Awake()
    {
        instance = this;
        menuPanel.SetActive(false);
    }
    

    public void ShowDeathMenu()
    {
        menuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}