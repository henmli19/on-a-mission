using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public PowerUp[] powerUpPrefabs;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;
    public float minSpawnTime = 20f;
    public float maxSpawnTime = 50f;

    private void Start()
    {
        SpawnNextPowerUp();
    }

    private void SpawnNextPowerUp()
    {
        if (powerUpPrefabs.Length == 0) return;

        // Spawn a random power-up
        int index = Random.Range(0, powerUpPrefabs.Length);
        Vector2 spawnPos = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        Instantiate(powerUpPrefabs[index], spawnPos, Quaternion.identity);

        // Random Zeit fuer naechstes Spawn
        float nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        Invoke(nameof(SpawnNextPowerUp), nextSpawnTime);
    }
}