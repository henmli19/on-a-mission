using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class NPCCutscene : MonoBehaviour
{
    public PlayableDirector cutscene;
    private bool playerNearby = false;
    private bool hasPlayed = false;

    public GameObject Canvas;
    public string nextSceneName;

    void Start()
    {
        Canvas.SetActive(false);

        if (cutscene != null)
            cutscene.stopped += OnCutsceneFinished;
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E) && !hasPlayed)
        {
            hasPlayed = true;

            cutscene.Play();
            Canvas.SetActive(true);
        }
    }

    void OnCutsceneFinished(PlayableDirector pd)
    {
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerNearby = false;
    }

    void OnDestroy()
    {
        if (cutscene != null)
            cutscene.stopped -= OnCutsceneFinished;
    }
}