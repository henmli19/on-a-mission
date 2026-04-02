using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using TMPro;

public class QuestNPC : MonoBehaviour
{
    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;
    public float interactRange = 2f;

    [Header("Timelines")]
    public PlayableDirector phase0Timeline;
    public PlayableDirector phase1Timeline;
    public PlayableDirector phase1bTimeline; 
    public PlayableDirector phase2Timeline;

    [Header("Enemy Spawning")]
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public int enemiesPerWave = 5;
    public Vector3 enemySpawnScale = new Vector3(0.5f, 0.5f, 1f);

    [Header("Kill Counter")]
    public TMP_Text killCounterText;
    public int totalEnemies = 10;

    [Header("Level Complete")]
    public GameObject levelCompleteUI;

    [Header("Memory Minigame")]
    public string memorySceneName = "MemoryGame";

    [Header("NPC Positions")]
    public Transform positionStart;
    public Transform positionAfterWave1;
    public Transform positionAfterWave2;

    [Header("NPC Visual")]
    public GameObject npcVisual;

    [Header("Game UI & Background")]
    public GameObject gameUI;         // your HUD canvas
    public GameObject gameBackground; // your level background object
    
    [Header("Camera")]
    public Camera mainCamera;
    
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

        if (levelCompleteUI != null)
            levelCompleteUI.SetActive(false);

        if (npcVisual != null)
            npcVisual.SetActive(true);

        if (positionStart != null)
            transform.position = positionStart.position;

        UpdateKillCounter();
    }

    // ─────────────────────────────────────────
    private void Update()
    {
        if (player == null || timelinePlaying)
            return;

        float dist = Vector2.Distance(transform.position, player.position);
        bool playerInRange = dist <= interactRange;

        if (playerInRange && IsNPCTalkable() && Input.GetKeyDown(interactKey))
            OnPlayerInteract();
    }

    private bool IsNPCTalkable()
    {
        return currentPhase == QuestPhase.NotStarted
            || currentPhase == QuestPhase.Wave1Complete
            || currentPhase == QuestPhase.Wave2Complete;
    }

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

    // ───── PHASE 0: Dialogue → Spawn Wave 1 ─────
    private IEnumerator Phase0Sequence()
    {
        timelinePlaying = true;

        if (phase0Timeline != null)
        {
            phase0Timeline.Play();
            yield return new WaitUntil(() => phase0Timeline.state != PlayState.Playing);
        }

        timelinePlaying = false;

        currentPhase = QuestPhase.Wave1Active;
        SpawnWave();
    }

    // ───── PHASE 1: Dialogue → Memory Minigame → Spawn Wave 2 ─────
    private IEnumerator Phase1Sequence()
    {
        timelinePlaying = true;

        // 1. Play dialogue and wait
        if (phase1Timeline != null)
        {
            phase1Timeline.Play();
            yield return new WaitUntil(() => phase1Timeline.state != PlayState.Playing);
        }

        // 2. Hide game UI and background before minigame loads
        if (gameUI != null) gameUI.SetActive(false);
        if (gameBackground != null) gameBackground.SetActive(false);

        // 3. Load minigame additively
        currentPhase = QuestPhase.MinigameActive;
        yield return SceneManager.LoadSceneAsync(memorySceneName, LoadSceneMode.Additive);

        // 4. Wait for minigame to finish
        yield return new WaitUntil(() => currentPhase != QuestPhase.MinigameActive);

        // 5. Unload minigame
        yield return SceneManager.UnloadSceneAsync(memorySceneName);

        // 6. Restore UI and background
        if (gameUI != null) gameUI.SetActive(true);
        if (gameBackground != null) gameBackground.SetActive(true);

        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
            mainCamera.gameObject.SetActive(true);
        }
        Camera.main?.gameObject.SetActive(true);

        timelinePlaying = false;

        // 7. NPC continues talking after minigame — MUST happen before spawn
        timelinePlaying = true;
        if (phase1bTimeline != null)
        {
            phase1bTimeline.Play();
            yield return new WaitUntil(() => phase1bTimeline.state != PlayState.Playing);
        }
        else
        {
            Debug.LogWarning("QuestNPC: phase1bTimeline is not assigned! Assign it in the Inspector.");
            yield return new WaitForSeconds(2f); // fallback delay so spawn isn't instant
        }
        timelinePlaying = false;

        // 8. Delay then spawn wave 2
        yield return new WaitForSeconds(1f);
        currentPhase = QuestPhase.Wave2Active;
        SpawnWave();
    }

    // ───── PHASE 2: Dialogue → Level Complete ─────
    private IEnumerator Phase2Sequence()
    {
        timelinePlaying = true;

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

    // ───── Called by CardController when minigame is complete ─────
    // Change OnMinigameComplete to use a proper intermediate phase
    public void OnMinigameComplete()
    {
        currentPhase = QuestPhase.Wave1Complete; // just needs to be != MinigameActive
    }

    // ───── ENEMY SPAWNING ─────
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
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = Instantiate(
                enemyPrefabs[Random.Range(0, enemyPrefabs.Length)],
                spawnPoint.position,
                Quaternion.identity
            );
            // No scale override — prefab size is used as-is

            QuestEnemy qe = enemy.GetComponent<QuestEnemy>();
            if (qe == null) qe = enemy.AddComponent<QuestEnemy>();
            qe.Init(this);
            activeEnemies++;
        }
    }

    // ───── Called by QuestEnemy when it dies ─────
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
                if (positionAfterWave1 != null)
                    transform.position = positionAfterWave1.position;
                Debug.Log("Wave 1 complete! Talk to the NPC.");
            }
            else if (currentPhase == QuestPhase.Wave2Active)
            {
                currentPhase = QuestPhase.Wave2Complete;
                if (positionAfterWave2 != null)
                    transform.position = positionAfterWave2.position;
                Debug.Log("Wave 2 complete! Talk to the NPC.");
            }
        }
    }

    // ───── Kill Counter UI ─────
    private void UpdateKillCounter()
    {
        if (killCounterText != null)
            killCounterText.text = $"[{totalKills}/{totalEnemies}] Enemies killed";
    }

    // ───── Gizmos ─────
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}