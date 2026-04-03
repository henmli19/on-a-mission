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

        // Disable player completely
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Stop all animations
            Animator anim = player.GetComponent<Animator>();
            if (anim != null) anim.enabled = false;

            // Disable all monobehaviour scripts on player
            foreach (MonoBehaviour script in player.GetComponents<MonoBehaviour>())
                script.enabled = false;
        }

        // Disable all other UI canvases so inventory etc cant open
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.gameObject != menuPanel && !menuPanel.transform.IsChildOf(canvas.transform))
                canvas.gameObject.SetActive(false);
        }
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