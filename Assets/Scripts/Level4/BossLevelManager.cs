using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using Player_Scripts;

public class BossLevelManager : MonoBehaviour
{
    [Header("Timelines")]
    public PlayableDirector introTimeline;
    public PlayableDirector postBossTimeline;

    [Header("Boss")]
    public GameObject bossObject;

    [Header("NPC")]
    public GameObject npcObject;
    public KeyCode interactKey = KeyCode.E;
    public float interactRange = 2f;
    public PlayableDirector npcTimeline;
    public GameObject npcCanvas;

    private enum LevelPhase
    {
        Intro,
        BossFight,
        PostBoss,
        NPCWaiting,
        NPCTalking,
        Done
    }

    private LevelPhase currentPhase = LevelPhase.Intro;
    private bool bossDefeated = false;
    private Transform player;
    private PlayerMovement playerMovement;
    private bool timelinePlaying = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
            playerMovement = player.GetComponent<PlayerMovement>();

        if (bossObject != null)
            bossObject.SetActive(false);

        if (npcObject != null)
            npcObject.SetActive(false);

        if (npcCanvas != null)
            npcCanvas.SetActive(false);

        StartCoroutine(LevelSequence());
    }

    private void Update()
    {
        if (currentPhase != LevelPhase.NPCWaiting) return;
        if (player == null || timelinePlaying) return;

        float checkDist = npcObject != null
            ? Vector2.Distance(npcObject.transform.position, player.position)
            : 0f;

        if (checkDist <= interactRange && Input.GetKeyDown(interactKey))
            StartCoroutine(NPCTalkSequence());
    }

    private IEnumerator PlayTimeline(PlayableDirector timeline)
    {
        if (timeline == null) yield break;
        timeline.Play();
        yield return new WaitForSeconds((float)timeline.duration);
    }

    private void RestorePlayerControls()
    {
        if (playerMovement != null)
            playerMovement.EnableControls();
    }

    private IEnumerator LevelSequence()
    {
        // ── PHASE 1: Intro Timeline ──
        timelinePlaying = true;
        currentPhase = LevelPhase.Intro;

        yield return StartCoroutine(PlayTimeline(introTimeline));
        RestorePlayerControls();

        timelinePlaying = false;

        CheckpointManager.Instance?.SetCheckpoint(player.position);

        // ── PHASE 2: Activate Boss ──
        currentPhase = LevelPhase.BossFight;

        if (bossObject != null)
        {
            bossObject.SetActive(true);

            BossDeathReporter reporter = bossObject.GetComponent<BossDeathReporter>();
            if (reporter == null)
                reporter = bossObject.AddComponent<BossDeathReporter>();

            reporter.Init(this);
        }
        else
        {
            Debug.LogWarning("BossLevelManager: No boss object assigned!");
            bossDefeated = true;
        }

        yield return new WaitUntil(() => bossDefeated);
        yield return new WaitForSeconds(1.5f);

        // ── PHASE 3: Post-Boss Timeline ──
        timelinePlaying = true;
        currentPhase = LevelPhase.PostBoss;

        yield return StartCoroutine(PlayTimeline(postBossTimeline));
        RestorePlayerControls();

        timelinePlaying = false;

        CheckpointManager.Instance?.SetCheckpoint(player.position);

        // ── PHASE 4: Show NPC ──
        currentPhase = LevelPhase.NPCWaiting;

        if (npcObject != null)
            npcObject.SetActive(true);
        else
            Debug.LogWarning("BossLevelManager: No NPC object assigned!");
    }

    private IEnumerator NPCTalkSequence()
    {
        timelinePlaying = true;
        currentPhase = LevelPhase.NPCTalking;

        if (npcCanvas != null) npcCanvas.SetActive(true);
        yield return StartCoroutine(PlayTimeline(npcTimeline));
        if (npcCanvas != null) npcCanvas.SetActive(false);

        RestorePlayerControls();

        timelinePlaying = false;
        currentPhase = LevelPhase.Done;
    }

    public void OnBossDefeated()
    {
        bossDefeated = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (npcObject == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(npcObject.transform.position, interactRange);
    }
}