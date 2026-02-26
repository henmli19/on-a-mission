using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    public void OpenLevel1()
    {
        SceneManager.LoadScene("TheBunkerScene");
    }

    public void OpenLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void OpenLevel3()
    {
        SceneManager.LoadScene("Level3");
    }

    public void OpenLevel4()
    {
        SceneManager.LoadScene("Level4");
    }
}