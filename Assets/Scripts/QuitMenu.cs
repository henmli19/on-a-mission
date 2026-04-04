using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuitMenu : MonoBehaviour
{
    public static QuitMenu instance;
    public GameObject menuPanel;
    public Button menuButton;
    public Button continueButton;

    void Awake()
    {
        instance = this;
        menuPanel.SetActive(false);
    }

    private void Start()
    {
        menuButton.onClick.AddListener(GoToMenu);
        continueButton.onClick.AddListener(Continue);
    }

    public void ShowDeathMenu()
    {
        Time.timeScale = 0f;
        menuPanel.SetActive(true);
    }

    void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    void Continue()
    {
        Time.timeScale = 1f;
        menuPanel.SetActive(false);

        if (CheckpointManager.Instance != null)
            CheckpointManager.Instance.RespawnPlayer();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}