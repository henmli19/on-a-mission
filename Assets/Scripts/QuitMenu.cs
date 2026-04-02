using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuitMenu : MonoBehaviour
{
    public static QuitMenu instance;
    public GameObject menuPanel;
    public Button menuButton;
    public Button restartButton;

    void Awake()
    {
        instance = this;
        menuPanel.SetActive(false);
    }

    private void Start()
    {
        menuButton.onClick.AddListener(GoToMenu);
        restartButton.onClick.AddListener(Restart);
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