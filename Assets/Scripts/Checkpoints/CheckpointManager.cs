using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [Header("Settings")]
    public int respawnHealth = 3;
    public Transform defaultSpawnPoint;

    private Vector3 currentCheckpoint;
    private bool checkpointSet = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentCheckpoint = defaultSpawnPoint != null
            ? defaultSpawnPoint.position
            : Vector3.zero;
    }

    public void SetCheckpoint(Vector3 position)
    {
        currentCheckpoint = position;
        checkpointSet = true;
        Debug.Log("CHECKPOINT SET at: " + position);
    }

    public void RespawnPlayer()
    {
        Debug.Log("RESPAWN CALLED | checkpointSet = " + checkpointSet + " | position = " + currentCheckpoint);

        if (!checkpointSet)
        {
            // No cutscene passed yet — full restart
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("CheckpointManager: Player not found!");
            return;
        }

        player.transform.position = currentCheckpoint;

        BatteryHealthUI health = player.GetComponent<BatteryHealthUI>();
        if (health != null) health.RespawnWithHealth(respawnHealth);

        Player_Scripts.PlayerMovement pm = player.GetComponent<Player_Scripts.PlayerMovement>();
        if (pm != null) pm.EnableControls();

        Animator anim = player.GetComponent<Animator>();
        if (anim != null) anim.enabled = true;
    }
}