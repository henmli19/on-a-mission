using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private float delay = 50f;

    void Start()
    {
        Invoke(nameof(LoadNextScene), delay);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}