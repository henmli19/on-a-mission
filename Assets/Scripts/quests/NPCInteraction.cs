using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class NPCQuestDialogue : MonoBehaviour
{
    [Header("Timelines")]
    public PlayableDirector introTimeline;
    public PlayableDirector returnWithItemTimeline;
    public PlayableDirector returnWithoutItemTimeline;

    [Header("Canvases")]
    public GameObject firstTimeCanvas;
    public GameObject withItemCanvas;
    public GameObject withoutItemCanvas;

    [Header("Settings")]
    public string requiredItemName = "Energy Core";
    public string nextSceneName;

    private bool playerNearby = false;
    private bool isPlaying = false;

    void Start()
    {
        firstTimeCanvas.SetActive(false);
        withItemCanvas.SetActive(false);
        withoutItemCanvas.SetActive(false);

        introTimeline.stopped += OnCutsceneFinished;
        returnWithItemTimeline.stopped += OnCutsceneFinished;
        returnWithoutItemTimeline.stopped += OnCutsceneFinished;
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E) && !isPlaying)
        {
            PlayCorrectTimeline();
        }
    }

    void PlayCorrectTimeline()
    {
        QuestManager qm = QuestManager.Instance;

        if (qm.energyCoreQuestState == QuestManager.QuestState.NotStarted)
        {
            qm.StartEnergyQuest();
            isPlaying = true;
            firstTimeCanvas.SetActive(true);
            introTimeline.Play();
            return;
        }

        if (qm.energyCoreQuestState == QuestManager.QuestState.Active)
        {
            if (PlayerHasItem())
            {
                qm.CompleteEnergyQuest();
                isPlaying = true;
                withItemCanvas.SetActive(true);
                returnWithItemTimeline.Play();
            }
            else
            {
                isPlaying = true;
                withoutItemCanvas.SetActive(true);
                returnWithoutItemTimeline.Play();
            }
        }
    }

    void OnCutsceneFinished(PlayableDirector pd)
    {
        firstTimeCanvas.SetActive(false);
        withItemCanvas.SetActive(false);
        withoutItemCanvas.SetActive(false);
        isPlaying = false;

        // If the timeline that just finished was the "with item" one, load next scene
        if (pd == returnWithItemTimeline)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private bool PlayerHasItem()
    {
        InventoryManager im = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        if (im == null) return false;

        foreach (ItemSlot slot in im.itemSlot)
        {
            if (slot.itemName == requiredItemName && slot.quantity > 0)
                return true;
        }
        return false;
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
        introTimeline.stopped -= OnCutsceneFinished;
        returnWithItemTimeline.stopped -= OnCutsceneFinished;
        returnWithoutItemTimeline.stopped -= OnCutsceneFinished;
    }
}