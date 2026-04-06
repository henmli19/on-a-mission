using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using TMPro;
using Player_Scripts;

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
    public PlayableDirector phase2OutroTimeline;

    [Header("Canvases")]
    public GameObject phase2OutroCanvas;

    [Header("Enemy Spawning")]
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public int enemiesPerWave = 5;

    [Header("Kill Counter")]
    public TMP_Text killCounterText;
    public int totalEnemies = 10;

    [Header("Level Complete")]
    public string nextSceneName = "Level4";

    [Header("Memory Minigame")]
    public string memorySceneName = "MemoryGame";

    [Header("NPC Positions")]
    public Transform positionStart;
    public Transform positionAfterWave1;
    public Transform positionAfterWave2;

    [Header("NPC Visual")]
    public GameObject npcVisual;

    [Header("Game UI & Background")]
    public GameObject gameUI;
    public GameObject gameBackground;

    [Header("Camera")]
    public Camera mainCamera;

    private enum QuestPhase
    {
        NotStarted,
        Wave1Active,
        Wave1Complete,
        MinigameActive,
        MinigameComplete,
        Wave2Active,
        Wave2Complete,
        LevelComplete
    }

    private QuestPhase currentPhase = QuestPhase.NotStarted;
    private int totalKills = 0;
    private int activeEnemies = 0;
    private bool timelinePlaying = false;
    private Transform player;
    private PlayerMovement playerMovement;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
            playerMovement = player.GetComponent<PlayerMovement>();

        if (npcVisual != null)
            npcVisual.SetActive(true);

        if (positionStart != null)
            transform.position = positionStart.position;

        if (phase2OutroCanvas != null)
            phase2OutroCanvas.SetActive(false);

        UpdateKillCounter();
    }

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

    // ───── PHASE 0: Dialogue → Spawn Wave 1 ─────
    private IEnumerator Phase0Sequence()
    {
        timelinePlaying = true;

        yield return StartCoroutine(PlayTimeline(phase0Timeline));
        RestorePlayerControls();

        timelinePlaying = false;

        CheckpointManager.Instance?.SetCheckpoint(player.position);

        currentPhase = QuestPhase.Wave1Active;
        SpawnWave();
    }

    // ───── PHASE 1: Dialogue → Minigame → Post-Minigame Dialogue → Spawn Wave 2 ─────
    private IEnumerator Phase1Sequence()
    {
        timelinePlaying = true;

        yield return StartCoroutine(PlayTimeline(phase1Timeline));
        RestorePlayerControls();

        if (gameUI != null) gameUI.SetActive(false);
        if (gameBackground != null) gameBackground.SetActive(false);

        currentPhase = QuestPhase.MinigameActive;
        yield return SceneManager.LoadSceneAsync(memorySceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => currentPhase == QuestPhase.MinigameComplete);
        yield return SceneManager.UnloadSceneAsync(memorySceneName);

        if (gameUI != null) gameUI.SetActive(true);
        if (gameBackground != null) gameBackground.SetActive(true);

        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
            mainCamera.gameObject.SetActive(true);
        }
        Camera.main?.gameObject.SetActive(true);

        yield return StartCoroutine(PlayTimeline(phase1bTimeline));
        RestorePlayerControls();

        timelinePlaying = false;

        CheckpointManager.Instance?.SetCheckpoint(player.position);

        yield return new WaitForSeconds(1f);
        currentPhase = QuestPhase.Wave2Active;
        SpawnWave();
    }

    // ───── PHASE 2: Dialogue → Outro Cutscene → Load Next Level ─────
    private IEnumerator Phase2Sequence()
    {
        timelinePlaying = true;

        yield return StartCoroutine(PlayTimeline(phase2Timeline));
        RestorePlayerControls();

        if (phase2OutroCanvas != null) phase2OutroCanvas.SetActive(true);
        yield return StartCoroutine(PlayTimeline(phase2OutroTimeline));
        if (phase2OutroCanvas != null) phase2OutroCanvas.SetActive(false);
        RestorePlayerControls();

        timelinePlaying = false;
        currentPhase = QuestPhase.LevelComplete;

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            Debug.LogWarning("QuestNPC: nextSceneName is not set in the Inspector!");
    }

    public void OnMinigameComplete()
    {
        currentPhase = QuestPhase.MinigameComplete;
    }

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

            QuestEnemy qe = enemy.GetComponent<QuestEnemy>();
            if (qe == null) qe = enemy.AddComponent<QuestEnemy>();
            qe.Init(this);
            activeEnemies++;
        }
    }

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

    private void UpdateKillCounter()
    {
        if (killCounterText != null)
            killCounterText.text = $"[{totalKills}/{totalEnemies}] Enemies killed";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}