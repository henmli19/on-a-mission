using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

// ─────────────────────────────────────────────────────────────
//  QuestNPC
//
//  Quest flow:
//  Phase 0 — Player talks to NPC → Timeline plays → 5 enemies spawn
//  Phase 1 — Player kills 5 enemies → talks to NPC → memory minigame
//           → 5 more enemies spawn
//  Phase 2 — Player kills 5 more enemies → talks to NPC → level complete
//
//  Setup:
//  - Attach this to your NPC GameObject
//  - Assign the PlayableDirector for each phase in the Inspector
//  - Assign your enemy prefab(s) and spawn points
//  - Assign the kill counter TMP text
//  - Assign your interaction prompt UI (optional)
// ─────────────────────────────────────────────────────────────
public class QuestNPC : MonoBehaviour
{
    // ── Interaction ───────────────────────────
    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;
    public float interactRange = 2f;
    public GameObject interactPromptUI;   // e.g. "Press E to talk" text

    // ── Timelines ─────────────────────────────
    [Header("Timelines")]
    [Tooltip("Plays when player first talks to NPC (Phase 0)")]
    public PlayableDirector phase0Timeline;
    [Tooltip("Plays when player talks again after killing first 5 enemies (Phase 1)")]
    public PlayableDirector phase1Timeline;
    [Tooltip("Plays when player talks after killing all 10 enemies (Phase 2 / level complete)")]
    public PlayableDirector phase2Timeline;

    // ── Enemy Spawning ────────────────────────
    [Header("Enemy Spawning")]
    public GameObject[] enemyPrefabs;     // assign one or more enemy prefabs
    public Transform[] spawnPoints;       // where enemies appear
    [Tooltip("How many enemies to spawn per wave")]
    public int enemiesPerWave = 5;

    // ── Kill Counter UI ───────────────────────
    [Header("Kill Counter")]
    public TMP_Text killCounterText;
    [Tooltip("Total enemies across both waves")]
    public int totalEnemies = 10;

    // ── Level Complete ────────────────────────
    [Header("Level Complete")]
    [Tooltip("Object or UI to activate when the level is complete")]
    public GameObject levelCompleteUI;

    // ── Memory Minigame ───────────────────────
    [Header("Memory Minigame")]
    [Tooltip("Assign your memory card minigame GameObject here")]
    public GameObject memoryMinigameObject;

    // ── Internal State ────────────────────────
    private enum QuestPhase
    {
        NotStarted,         // player hasn't talked to NPC yet
        Wave1Active,        // 5 enemies alive, player must kill them
        Wave1Complete,      // all 5 dead, waiting for player to talk
        MinigameActive,     // memory minigame is running
        Wave2Active,        // 5 more enemies alive
        Wave2Complete,      // all 10 dead, waiting for player to talk
        LevelComplete       // quest done
    }

    private QuestPhase currentPhase = QuestPhase.NotStarted;
    private int totalKills = 0;
    private int activeEnemies = 0;
    private bool playerInRange = false;
    private bool timelinePlaying = false;
    private Transform player;

    // ─────────────────────────────────────────
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Hide interact prompt at start
        if (interactPromptUI != null)
            interactPromptUI.SetActive(false);

        // Hide level complete UI
        if (levelCompleteUI != null)
            levelCompleteUI.SetActive(false);

        // Make sure minigame is hidden
        if (memoryMinigameObject != null)
            memoryMinigameObject.SetActive(false);

        UpdateKillCounter();
    }

    // ─────────────────────────────────────────
    private void Update()
    {
        if (player == null || timelinePlaying) return;

        float dist = Vector2.Distance(transform.position, player.position);
        playerInRange = dist <= interactRange;

        // Show or hide the interact prompt
        bool canTalk = playerInRange && IsNPCTalkable();
        if (interactPromptUI != null)
            interactPromptUI.SetActive(canTalk);

        // Handle interact key press
        if (canTalk && Input.GetKeyDown(interactKey))
            OnPlayerInteract();
    }

    // ── Returns true when the NPC can be talked to ──
    private bool IsNPCTalkable()
    {
        return currentPhase == QuestPhase.NotStarted
            || currentPhase == QuestPhase.Wave1Complete
            || currentPhase == QuestPhase.Wave2Complete;
    }

    // ─────────────────────────────────────────
    //  Interaction Handler
    // ─────────────────────────────────────────
    private void OnPlayerInteract()
    {
        switch (currentPhase)
        {
            case QuestPhase.NotStarted:
                StartCoroutine(Phase0Sequence());
                break;

            case QuestPhase.Wave1Complete:
                StartCoroutine(Phase1Sequence());
                break;

            case QuestPhase.Wave2Complete:
                StartCoroutine(Phase2Sequence());
                break;
        }
    }

    // ─────────────────────────────────────────
    //  PHASE 0 — First talk → Timeline → Wave 1
    // ─────────────────────────────────────────
    private IEnumerator Phase0Sequence()
    {
        timelinePlaying = true;
        if (interactPromptUI != null) interactPromptUI.SetActive(false);

        // Play phase 0 timeline
        if (phase0Timeline != null)
        {
            phase0Timeline.Play();
            yield return new WaitUntil(() =>
                phase0Timeline.state != PlayState.Playing);
        }

        timelinePlaying = false;

        // Spawn first wave
        SpawnWave();
        currentPhase = QuestPhase.Wave1Active;
        UpdateKillCounter();
    }

    // ─────────────────────────────────────────
    //  PHASE 1 — Second talk → Timeline → Minigame → Wave 2
    // ─────────────────────────────────────────
    private IEnumerator Phase1Sequence()
    {
        timelinePlaying = true;
        if (interactPromptUI != null) interactPromptUI.SetActive(false);

        currentPhase = QuestPhase.MinigameActive;

        // Play phase 1 timeline
        if (phase1Timeline != null)
        {
            phase1Timeline.Play();
            yield return new WaitUntil(() =>
                phase1Timeline.state != PlayState.Playing);
        }

        // ── MEMORY MINIGAME ──────────────────
        // The minigame is activated here.
        // When your minigame finishes, it should call
        // QuestNPC.OnMinigameComplete() on this component
        // so the quest can continue.
        // Example from your minigame script:
        //   FindObjectOfType<QuestNPC>().OnMinigameComplete();
        if (memoryMinigameObject != null)
        {
            memoryMinigameObject.SetActive(true);

            // Wait until your minigame calls OnMinigameComplete()
            yield return new WaitUntil(() => currentPhase != QuestPhase.MinigameActive);
        }
        else
        {
            // No minigame assigned — skip straight to wave 2
            Debug.LogWarning("QuestNPC: No memory minigame assigned, skipping.");
            currentPhase = QuestPhase.Wave2Active;
        }
        // ── END MEMORY MINIGAME ──────────────

        timelinePlaying = false;

        // Spawn second wave
        SpawnWave();
        currentPhase = QuestPhase.Wave2Active;
        UpdateKillCounter();
    }

    // ─────────────────────────────────────────
    //  PHASE 2 — Final talk → Timeline → Level Complete
    // ─────────────────────────────────────────
    private IEnumerator Phase2Sequence()
    {
        timelinePlaying = true;
        if (interactPromptUI != null) interactPromptUI.SetActive(false);

        // Play phase 2 timeline
        if (phase2Timeline != null)
        {
            phase2Timeline.Play();
            yield return new WaitUntil(() =>
                phase2Timeline.state != PlayState.Playing);
        }

        timelinePlaying = false;
        currentPhase = QuestPhase.LevelComplete;

        // Show level complete UI
        if (levelCompleteUI != null)
            levelCompleteUI.SetActive(true);

        Debug.Log("Level Complete!");
    }

    // ─────────────────────────────────────────
    //  Called by your memory minigame when it finishes
    //  Add this call to the end of your minigame script:
    //      FindObjectOfType<QuestNPC>().OnMinigameComplete();
    // ─────────────────────────────────────────
    public void OnMinigameComplete()
    {
        if (memoryMinigameObject != null)
            memoryMinigameObject.SetActive(false);

        // Unlocks the WaitUntil in Phase1Sequence
        currentPhase = QuestPhase.Wave2Active;
    }

    // ─────────────────────────────────────────
    //  Enemy Spawning
    // ─────────────────────────────────────────
    private void SpawnWave()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("QuestNPC: No enemy prefabs assigned!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("QuestNPC: No spawn points assigned!");
            return;
        }

        activeEnemies = 0;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            // Pick a random spawn point and random enemy prefab
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject prefab    = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

            // Attach the death reporter so we know when this enemy dies
            QuestEnemy questEnemy = enemy.GetComponent<QuestEnemy>();
            if (questEnemy == null)
                questEnemy = enemy.AddComponent<QuestEnemy>();

            questEnemy.Init(this);
            activeEnemies++;
        }
    }

    // ─────────────────────────────────────────
    //  Called by QuestEnemy when it dies
    // ─────────────────────────────────────────
    public void OnQuestEnemyKilled()
    {
        totalKills++;
        activeEnemies--;
        UpdateKillCounter();

        // Check if this wave is done
        if (activeEnemies <= 0)
        {
            if (currentPhase == QuestPhase.Wave1Active)
            {
                currentPhase = QuestPhase.Wave1Complete;
                Debug.Log("Wave 1 complete! Talk to the NPC.");
            }
            else if (currentPhase == QuestPhase.Wave2Active)
            {
                currentPhase = QuestPhase.Wave2Complete;
                Debug.Log("Wave 2 complete! Talk to the NPC.");
            }
        }
    }

    // ─────────────────────────────────────────
    //  Kill Counter UI
    // ─────────────────────────────────────────
    private void UpdateKillCounter()
    {
        if (killCounterText != null)
            killCounterText.text = "[" + totalKills + "/" + totalEnemies + "] Enemies killed";
    }

    // ─────────────────────────────────────────
    //  Gizmos — shows interact range in Scene view
    // ─────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
