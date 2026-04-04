using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

// ─────────────────────────────────────────────────────────────
//  BossLevelManager
//
//  Level flow:
//  1. Intro Timeline  — player talks to themselves
//  2. Boss spawns / activates
//  3. Player kills boss
//  4. Post-Boss Timeline — player talks to themselves again
//  5. NPC appears — player presses E to trigger NPC Timeline
//
//  Setup:
//  - Create an empty GameObject in the scene and attach this
//  - Assign all fields in the Inspector
//  - Make sure your Boss GameObject is in the scene but INACTIVE
//    at start (we activate it after the intro timeline)
//  - BossDeathReporter is added automatically to the boss at runtime
// ─────────────────────────────────────────────────────────────
public class BossLevelManager : MonoBehaviour
{
    // ── Timelines ─────────────────────────────
    [Header("Timelines")]
    [Tooltip("Player talks to themselves before the boss fight")]
    public PlayableDirector introTimeline;
    [Tooltip("Player talks to themselves after the boss is defeated")]
    public PlayableDirector postBossTimeline;

    // ── Boss ──────────────────────────────────
    [Header("Boss")]
    [Tooltip("The Boss GameObject — should be INACTIVE in the scene at start")]
    public GameObject bossObject;

    // ── NPC ───────────────────────────────────
    [Header("NPC")]
    [Tooltip("The NPC GameObject — hidden until after the post-boss dialogue")]
    public GameObject npcObject;
    public KeyCode interactKey = KeyCode.E;
    public float interactRange = 2f;
    [Tooltip("Timeline that plays when the player talks to the NPC")]
    public PlayableDirector npcTimeline;

    // ── Internal State ────────────────────────
    private enum LevelPhase
    {
        Intro,          // intro timeline playing
        BossFight,      // boss is alive
        PostBoss,       // post-boss timeline playing
        NPCWaiting,     // waiting for player to talk to NPC
        NPCTalking,     // NPC timeline playing
        Done            // level complete
    }

    private LevelPhase currentPhase = LevelPhase.Intro;
    private bool bossDefeated = false;
    private Transform player;
    private bool timelinePlaying = false;

    // ─────────────────────────────────────────
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Boss starts inactive
        if (bossObject != null)
            bossObject.SetActive(false);

        // NPC starts hidden
        if (npcObject != null)
            npcObject.SetActive(false);

        StartCoroutine(LevelSequence());
    }

    // ─────────────────────────────────────────
    private void Update()
    {
        if (currentPhase != LevelPhase.NPCWaiting) return;
        if (player == null || timelinePlaying) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // Use NPC position for range check if available
        float checkDist = npcObject != null
            ? Vector2.Distance(npcObject.transform.position, player.position)
            : dist;

        if (checkDist <= interactRange && Input.GetKeyDown(interactKey))
            StartCoroutine(NPCTalkSequence());
    }

    // ─────────────────────────────────────────
    //  Main Level Sequence
    // ─────────────────────────────────────────
    private IEnumerator LevelSequence()
    {
        // ── PHASE 1: Intro Timeline ──
        timelinePlaying = true;
        currentPhase = LevelPhase.Intro;

        if (introTimeline != null)
        {
            introTimeline.Play();
            yield return new WaitUntil(() => introTimeline.state != PlayState.Playing);
        }

        timelinePlaying = false;

        // ── PHASE 2: Activate Boss ──
        currentPhase = LevelPhase.BossFight;

        if (bossObject != null)
        {
            bossObject.SetActive(true);

            // Attach the death reporter so we know when the boss dies
            BossDeathReporter reporter = bossObject.GetComponent<BossDeathReporter>();
            if (reporter == null)
                reporter = bossObject.AddComponent<BossDeathReporter>();

            reporter.Init(this);
        }
        else
        {
            Debug.LogWarning("BossLevelManager: No boss object assigned! Skipping boss phase.");
            bossDefeated = true;
        }

        // Wait until the boss is destroyed
        yield return new WaitUntil(() => bossDefeated);

        // Small pause so the boss death feels weighty
        yield return new WaitForSeconds(1.5f);

        // ── PHASE 3: Post-Boss Timeline ──
        timelinePlaying = true;
        currentPhase = LevelPhase.PostBoss;

        if (postBossTimeline != null)
        {
            postBossTimeline.Play();
            yield return new WaitUntil(() => postBossTimeline.state != PlayState.Playing);
        }

        timelinePlaying = false;

        // ── PHASE 4: Show NPC ──
        currentPhase = LevelPhase.NPCWaiting;

        if (npcObject != null)
            npcObject.SetActive(true);
        else
            Debug.LogWarning("BossLevelManager: No NPC object assigned!");
    }

    // ─────────────────────────────────────────
    //  NPC Talk Sequence
    // ─────────────────────────────────────────
    private IEnumerator NPCTalkSequence()
    {
        timelinePlaying = true;
        currentPhase = LevelPhase.NPCTalking;

        if (npcTimeline != null)
        {
            npcTimeline.Play();
            yield return new WaitUntil(() => npcTimeline.state != PlayState.Playing);
        }

        timelinePlaying = false;
        currentPhase = LevelPhase.Done;
    }

    // ─────────────────────────────────────────
    //  Called by BossDeathReporter when boss dies
    // ─────────────────────────────────────────
    public void OnBossDefeated()
    {
        bossDefeated = true;
    }

            // ─────────────────────────────────────────
            //  Gizmos — NPC interact range
            // ─────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        if (npcObject == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(npcObject.transform.position, interactRange);
    }
}