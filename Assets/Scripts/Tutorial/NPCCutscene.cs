using UnityEngine;
using UnityEngine.Playables;

public class NPCCutscene : MonoBehaviour
{
    public PlayableDirector cutscene;  

    private bool playerNearby = false;
    public GameObject Canvas;

    void Start()
    {
        Canvas.SetActive(false);
    }
    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            cutscene.Play();
            Canvas.SetActive(true);   
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}