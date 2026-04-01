using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class QuestNPC : MonoBehaviour
{
    // ── Interaction ───────────────────────────
    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;
    public float interactRange = 2f;
    public GameObject interactPromptUI;

    // ── Timelines ─────────────────────────────
    [Header("Timelines")]
    public PlayableDirector phase0Timeline;
    public PlayableDirector phase1Timeline;
    public PlayableDirector phase2Timeline;

    // ── Enemy Spawning ────────────────────────
    [Header("Enemy Spawning")]
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public int enemiesPerWave = 5;

    // ── Kill Counter UI ───────────────────────
    [Header("Kill Counter")]
    public TMP_Text killCounterText;
    public int totalEnemies = 10;

    // ── Level Complete ────────────────────────
    [Header("Level Complete")]
    public GameObject levelCompleteUI;

    // ── Memory Minigame ───────────────────────
    [Header("Memory Minigame")]
    public GameObject memoryMinigameObject;

    // ── NPC Movement ──────────────────────────
    [Header("NPC Positions")]
    public Transform positionStart;
    public Transform positionAfterWave1;
    public Transform positionAfterWave2;

    [Header("NPC Visual")]
    public GameObject npcVisual;
    public float disappearDelay = 0.5f;

    // ── Internal State ────────────────────────
    private enum QuestPhase
    {
        NotStarted,
        Wave1Active,
        Wave1Complete,
        MinigameActive,
        Wave2Active,
        Wave2Complete,
        LevelComplete
    }

    private QuestPhase currentPhase = QuestPhase.NotStarted;
    private int totalKills = 0;
    private int activeEnemies = 0;
    private bool timelinePlaying = false;
    private Transform player;

    // ─────────────────────────────────────────
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (phase0Timeline != null) phase0Timeline.Stop();
        if (phase1Timeline != null) phase1Timeline.Stop();
        if (phase2Timeline != null) phase2Timeline.Stop();

        if (interactPromptUI != null) interactPromptUI.SetActive(false);
        if (levelCompleteUI != null) levelCompleteUI.SetActive(false);
        if (memoryMinigameObject != null) memoryMinigameObject.SetActive(false);

        // Start position
        if (positionStart != null)
            transform.position = positionStart.position;

        UpdateKillCounter();
    }

    // ─────────────────────────────────────────
    private void Update()
    {
        if (player == null || timelinePlaying || npcVisual == null || !npcVisual.activeSelf)
            return;

        float dist = Vector2.Distance(transform.position, player.position);
        bool playerInRange = dist <= interactRange;

        bool canTalk = playerInRange && IsNPCTalkable();

        if (interactPromptUI != null)
            interactPromptUI.SetActive(canTalk);

        if (canTalk && Input.GetKeyDown(interactKey))
            OnPlayerInteract();
    }

    private bool IsNPCTalkable()
    {
        return currentPhase == QuestPhase.NotStarted
            || currentPhase == QuestPhase.Wave1Complete
            || currentPhase == QuestPhase.Wave2Complete;
    }

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
    private IEnumerator Phase0Sequence()
    {
        timelinePlaying = true;
        if (interactPromptUI != null) interactPromptUI.SetActive(false);

        if (phase0Timeline != null)
        {
            phase0Timeline.Play();
            yield return new WaitUntil(() => phase0Timeline.state != PlayState.Playing);
        }

        timelinePlaying = false;

        StartCoroutine(Disappear());

        SpawnWave();
        currentPhase = QuestPhase.Wave1Active;
        UpdateKillCounter();
    }

    // ─────────────────────────────────────────
    private IEnumerator Phase1Sequence()
    {
        timelinePlaying = true;
        if (interactPromptUI != null) interactPromptUI.SetActive(false);

        currentPhase = QuestPhase.MinigameActive;

        if (phase1Timeline != null)
        {
            phase1Timeline.Play();
            yield return new WaitUntil(() => phase1Timeline.state != PlayState.Playing);
        }

        if (memoryMinigameObject != null)
        {
            memoryMinigameObject.SetActive(true);
            yield return new WaitUntil(() => currentPhase != QuestPhase.MinigameActive);
        }
        else
        {
            currentPhase = QuestPhase.Wave2Active;
        }

        timelinePlaying = false;

        StartCoroutine(Disappear());

        SpawnWave();
        currentPhase = QuestPhase.Wave2Active;
        UpdateKillCounter();
    }

    // ─────────────────────────────────────────
    private IEnumerator Phase2Sequence()
    {
        timelinePlaying = true;
        if (interactPromptUI != null) interactPromptUI.SetActive(false);

        if (phase2Timeline != null)
        {
            phase2Timeline.Play();
            yield return new WaitUntil(() => phase2Timeline.state != PlayState.Playing);
        }

        timelinePlaying = false;
        currentPhase = QuestPhase.LevelComplete;

        if (levelCompleteUI != null)
            levelCompleteUI.SetActive(true);
    }

    // ─────────────────────────────────────────
    public void OnMinigameComplete()
    {
        if (memoryMinigameObject != null)
            memoryMinigameObject.SetActive(false);

        currentPhase = QuestPhase.Wave2Active;
    }

    // ─────────────────────────────────────────
    private void SpawnWave()
    {
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        activeEnemies = 0;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

            QuestEnemy qe = enemy.GetComponent<QuestEnemy>();
            if (qe == null)
                qe = enemy.AddComponent<QuestEnemy>();

            qe.Init(this);
            activeEnemies++;
        }
    }

    // ─────────────────────────────────────────
    public void OnQuestEnemyKilled()
    {
        totalKills++;
        activeEnemies--;
        UpdateKillCounter();

        if (activeEnemies <= 0)
        {
            if (currentPhase == QuestPhase.Wave1Active)
            {
                currentPhase = QuestPhase.Wave1Complete;
                AppearAt(positionAfterWave1);
            }
            else if (currentPhase == QuestPhase.Wave2Active)
            {
                currentPhase = QuestPhase.Wave2Complete;
                AppearAt(positionAfterWave2);
            }
        }
    }

    // ─────────────────────────────────────────
    private IEnumerator Disappear()
    {
        yield return new WaitForSeconds(disappearDelay);
        npcVisual.SetActive(false);
    }

    private void AppearAt(Transform target)
    {
        if (target == null) return;

        transform.position = target.position;
        npcVisual.SetActive(true);
    }

    // ─────────────────────────────────────────
    private void UpdateKillCounter()
    {
        if (killCounterText != null)
            killCounterText.text = "[" + totalKills + "/" + totalEnemies + "] Enemies killed";
    }
} 
