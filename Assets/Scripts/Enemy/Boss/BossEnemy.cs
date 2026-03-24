using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ─────────────────────────────────────────────
//  BossEnemy  –  Sword-wielding humanoid boss
//  Moves:
//    1) Minion Summon  (DroneLaser + SphereEnemy)
//    2) Sword Slash    (melee lunge arc)
//    3) Radial Burst   (ring of projectiles)
//    4) Teleport       (vanish + reappear around player + slash)
// ─────────────────────────────────────────────
public class BossEnemy : Enemy
{
    // ── Phase thresholds ──────────────────────
    [Header("Phase Thresholds")]
    [Tooltip("Boss becomes Enraged below this HP fraction (0-1)")]
    public float enrageThreshold = 0.4f;
    private bool isEnraged = false;

    // ── Minion Summon ─────────────────────────
    [Header("Minion Summon")]
    public GameObject dronePrefab;
    public GameObject spherePrefab;
    public Transform[] spawnPoints;
    public int minionsPerSummon = 2;
    public float summonCooldown = 12f;
    public PowerupDropTable powerupDropTable;

    // Key = minion GameObject, Value = last known world position
    private Dictionary<GameObject, Vector2> trackedMinions = new Dictionary<GameObject, Vector2>();

    // ── Sword Slash ───────────────────────────
    [Header("Sword Slash")]
    public float slashRange = 1.8f;
    public float slashDamage = 25f;
    public float slashCooldown = 2.5f;
    public float lungeDuration = 0.2f;
    public Transform slashHitbox;
    private float nextSlashTime;

    // ── Radial Burst ──────────────────────────
    [Header("Radial Burst")]
    public GameObject bossBulletPrefab;
    public Transform firePoint;
    public int burstProjectileCount = 6;
    public float burstProjectileSpeed = 7f;
    public float burstCooldown = 6f;
    public int burstWaves = 2;
    public float burstWaveDelay = 0.25f;
    private float nextBurstTime;
    private float nextSummonTime;

    // ── Teleport ──────────────────────────────
    [Header("Teleport")]
    public float teleportCooldown = 8f;
    [Tooltip("How far from the player the boss reappears")]
    public float teleportRange = 3f;
    [Tooltip("Optional puff/flash effect spawned on vanish and arrival")]
    public GameObject teleportVFXPrefab;
    private float nextTeleportTime;
    public float backawayDistance = 3f; // if player gets closer than this, boss teleports away

    // ── State ─────────────────────────────────
    private enum BossState { Idle, Chasing, Summoning, Slashing, Bursting, Teleporting }
    private BossState currentState = BossState.Idle;
    private bool isActing = false;

    // ── Boss Health Bar ────────────────────────
    private BossHealthBar bossHealthBar;

    // ── Facing / Visuals ──────────────────────
    private SpriteRenderer sr;

    // ─────────────────────────────────────────
    protected override void Start()
    {
        base.Start();

        sr = GetComponent<SpriteRenderer>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        bossHealthBar = FindObjectOfType<BossHealthBar>();
        if (bossHealthBar != null)
            bossHealthBar.SetMaxHealth(maxHealth);

        // Boost base move speed right from the start
        moveSpeed *= 1.4f;

        nextSummonTime   = Time.time + summonCooldown * 0.5f;
        nextSlashTime    = Time.time + 1f;
        nextBurstTime    = Time.time + 3f;
        nextTeleportTime = Time.time + teleportCooldown;
    }

    // ─────────────────────────────────────────
    protected override void Update()
    {
        if (isDead || player == null) return;

        // Always face the player regardless of what the boss is doing
        FacePlayer();

        // If summoning and player gets too close, teleport to the other side
        if (currentState == BossState.Summoning && player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance < backawayDistance && !isActing)
            {
                StartCoroutine(BackawayTeleport());
                return;
            }
        }
        
        if (isActing) return;

        // Check enrage
        if (!isEnraged && currentHealth / maxHealth <= enrageThreshold)
            TriggerEnrage();

        float dist = Vector2.Distance(transform.position, player.position);

        // ── Priority order ──────────────────

        // 1. Summon — only if no minions are currently alive
        if (Time.time >= nextSummonTime && trackedMinions.Count == 0)
        {
            StartCoroutine(SummonMinions());
            return;
        }

        // 2. Teleport — medium range, before slash
        if (dist <= detectionRange && dist > slashRange && Time.time >= nextTeleportTime)
        {
            StartCoroutine(TeleportAroundPlayer());
            return;
        }

        // 3. Slash — close range
        if (dist <= slashRange && Time.time >= nextSlashTime)
        {
            StartCoroutine(SwordSlash());
            return;
        }

        // 4. Radial Burst — medium range
        if (dist <= detectionRange && Time.time >= nextBurstTime)
        {
            StartCoroutine(RadialBurst());
            return;
        }

        // 5. Chase / Patrol
        if (dist <= detectionRange)
            ChasePlayer();
        else
            Patrol();
    }

    // ─── Always face the player ───────────────
    private void FacePlayer()
    {
        if (sr == null || player == null) return;
        sr.flipX = player.position.x > transform.position.x; // was <, now >
    }

    // ─────────────────────────────────────────
    //  MOVE 1 – Minion Summon
    // ─────────────────────────────────────────
    private IEnumerator SummonMinions()
    {
        isActing = true;
        currentState = BossState.Summoning;
        nextSummonTime = Time.time + summonCooldown;

        yield return new WaitForSeconds(0.6f);

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("BossEnemy: No spawn points assigned!");
            isActing = false;
            yield break;
        }

        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        for (int i = 0; i < minionsPerSummon; i++)
        {
            if (availablePoints.Count == 0) break;

            int idx = Random.Range(0, availablePoints.Count);
            Transform spawnPoint = availablePoints[idx];
            availablePoints.RemoveAt(idx);

            GameObject prefab = (i % 2 == 0) ? dronePrefab : spherePrefab;
            if (prefab == null) continue;

            GameObject minion = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

            // Track this minion — drop is handled by WatchMinions()
            trackedMinions[minion] = spawnPoint.position;

            yield return new WaitForSeconds(0.15f);
        }

        // Watch for minion deaths in the background
        StartCoroutine(WatchMinions());

        // Wait until all minions are dead before unlocking boss actions
        yield return new WaitUntil(() => trackedMinions.Count == 0);

        currentState = BossState.Idle;
        isActing = false;
    }

    // ─────────────────────────────────────────
    //  Minion Death Watcher
    //  Tracks each minion's position every frame.
    //  The moment one goes null it spawns the drop.
    // ─────────────────────────────────────────
    private IEnumerator WatchMinions()
    {
        while (trackedMinions.Count > 0)
        {
            // Copy keys into a separate list so we can safely
            // modify the dictionary while iterating
            List<GameObject> keys = new List<GameObject>(trackedMinions.Keys);

            foreach (GameObject minion in keys)
            {
                if (minion == null)
                {
                    // Minion just died — spawn drop at last recorded position
                    SpawnDrop(trackedMinions[minion]);
                    trackedMinions.Remove(minion);
                }
                else
                {
                    // Still alive — update last known position
                    trackedMinions[minion] = minion.transform.position;
                }
            }

            yield return null;
        }
    }

    // ─────────────────────────────────────────
    private void SpawnDrop(Vector2 position)
    {
        if (powerupDropTable == null)
        {
            Debug.LogWarning("BossEnemy: PowerupDropTable is not assigned!");
            return;
        }

        GameObject drop = powerupDropTable.GetRandomDrop();
        if (drop != null)
        {
            // Always spawn at Y = 3, keep the minion's X position
            Vector2 dropPosition = new Vector2(position.x, 3f);
            Instantiate(drop, dropPosition, Quaternion.identity);
            Debug.Log("Powerup dropped at " + dropPosition);
        }
        else
        {
            Debug.LogWarning("BossEnemy: GetRandomDrop() returned null — check your drop table entries.");
        }
    }

    // ─────────────────────────────────────────
    //  MOVE 2 – Sword Slash
    // ─────────────────────────────────────────
    private IEnumerator SwordSlash()
    {
        isActing = true;
        currentState = BossState.Slashing;
        nextSlashTime = Time.time + slashCooldown;

        yield return new WaitForSeconds(0.3f);

        Vector2 lungeDir = (player.position - transform.position).normalized;
        float elapsed = 0f;
        float lungeSpeed = slashRange / lungeDuration * 2f;

        while (elapsed < lungeDuration)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                (Vector2)transform.position + lungeDir,
                lungeSpeed * Time.deltaTime
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        Vector2 hitOrigin = slashHitbox != null
            ? (Vector2)slashHitbox.position
            : (Vector2)transform.position;

        Collider2D hit = Physics2D.OverlapCircle(hitOrigin, slashRange * 0.8f, playerLayer);
        if (hit != null)
        {
            BatteryHealthUI playerHealth = hit.GetComponent<BatteryHealthUI>();
            if (playerHealth != null)
                playerHealth.TakeDamage((int)slashDamage);
        }

        yield return new WaitForSeconds(0.4f);

        currentState = BossState.Idle;
        isActing = false;
    }

    // ─────────────────────────────────────────
    //  MOVE 3 – Radial Burst
    // ─────────────────────────────────────────
    private IEnumerator RadialBurst()
    {
        isActing = true;
        currentState = BossState.Bursting;
        nextBurstTime = Time.time + burstCooldown;

        yield return new WaitForSeconds(0.5f);

        Vector2 origin = firePoint != null
            ? (Vector2)firePoint.position
            : (Vector2)transform.position;

        for (int wave = 0; wave < burstWaves; wave++)
        {
            float angleOffset = wave * (360f / burstProjectileCount / burstWaves);
            FireRing(origin, angleOffset);

            if (wave < burstWaves - 1)
                yield return new WaitForSeconds(burstWaveDelay);
        }

        yield return new WaitForSeconds(0.6f);

        currentState = BossState.Idle;
        isActing = false;
    }

    private void FireRing(Vector2 origin, float angleOffset)
    {
        if (bossBulletPrefab == null) return;

        float step = 360f / burstProjectileCount;

        for (int i = 0; i < burstProjectileCount; i++)
        {
            float angle      = i * step + angleOffset;
            float rad        = angle * Mathf.Deg2Rad;
            Vector2 dir      = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            Vector2 spawnPos = origin + dir * 2.5f; // spread bullets far apart

            GameObject bullet = Instantiate(bossBulletPrefab, spawnPos, Quaternion.identity);
            LaserProjectile proj = bullet.GetComponent<LaserProjectile>();
            if (proj != null)
                proj.SetDirection(dir, burstProjectileSpeed);
        }
    }

    // ─────────────────────────────────────────
    //  MOVE 4 – Teleport Around Player
    // ─────────────────────────────────────────
    private IEnumerator TeleportAroundPlayer()
    {
        isActing = true;
        currentState = BossState.Teleporting;
        nextTeleportTime = Time.time + teleportCooldown;

        // Spawn VFX at current position before vanishing
        if (teleportVFXPrefab != null)
            Instantiate(teleportVFXPrefab, transform.position, Quaternion.identity);

        // Hide the boss visually
        if (sr != null) sr.enabled = false;

        yield return new WaitForSeconds(0.4f);

        // Pick a random position around the player
        Vector2[] offsets = new Vector2[]
        {
            new Vector2(-teleportRange, 0),
            new Vector2( teleportRange, 0),
            new Vector2(-teleportRange * 0.7f,  teleportRange * 0.7f),
            new Vector2( teleportRange * 0.7f,  teleportRange * 0.7f),
        };

        Vector2 chosenOffset   = offsets[Random.Range(0, offsets.Length)];
        Vector2 teleportTarget = (Vector2)player.position + chosenOffset;

        transform.position = teleportTarget;

        // Spawn VFX at new position on arrival
        if (teleportVFXPrefab != null)
            Instantiate(teleportVFXPrefab, transform.position, Quaternion.identity);

        // Reappear
        if (sr != null) sr.enabled = true;

        // Brief pause so player can react before the follow-up slash
        yield return new WaitForSeconds(0.15f);

        currentState = BossState.Idle;
        isActing = false;

        // Immediately follow up with a slash if close enough
        if (Vector2.Distance(transform.position, player.position) <= slashRange * 1.5f)
            StartCoroutine(SwordSlash());
    }
    
    private IEnumerator BackawayTeleport()
    {
        isActing = true;

        if (teleportVFXPrefab != null)
            Instantiate(teleportVFXPrefab, transform.position, Quaternion.identity);

        if (sr != null) sr.enabled = false;

        yield return new WaitForSeconds(0.25f);

        // Teleport to the opposite side of the player from where the boss currently is
        Vector2 directionFromPlayer = ((Vector2)transform.position - (Vector2)player.position).normalized;
        Vector2 oppositePos = (Vector2)player.position - directionFromPlayer * teleportRange;

        transform.position = oppositePos;

        if (teleportVFXPrefab != null)
            Instantiate(teleportVFXPrefab, transform.position, Quaternion.identity);

        if (sr != null) sr.enabled = true;

        yield return new WaitForSeconds(0.1f);

        isActing = false;
        currentState = BossState.Summoning; // go back to waiting for minions
    }

    // ─────────────────────────────────────────
    //  Enrage — faster + shorter cooldowns
    // ─────────────────────────────────────────
    private void TriggerEnrage()
    {
        isEnraged        = true;
        moveSpeed        *= 1.5f;
        slashCooldown    *= 0.6f;
        burstCooldown    *= 0.6f;
        summonCooldown   *= 0.7f;
        teleportCooldown *= 0.7f; // teleports more frequently when enraged
        burstWaves++;

        Debug.Log("Boss ENRAGED!");
    }

    // ─────────────────────────────────────────
    //  Damage & Death
    // ─────────────────────────────────────────
    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        if (bossHealthBar != null)
            bossHealthBar.SetHealth(currentHealth);
    }

    protected override void Die()
    {
        isDead = true;

        if (bossHealthBar != null)
            bossHealthBar.HideBar();

        // Kill all remaining tracked minions
        foreach (var entry in trackedMinions)
            if (entry.Key != null) Destroy(entry.Key);

        trackedMinions.Clear();

        Debug.Log("Boss defeated!");
        Destroy(gameObject, 1.5f);
    }

    protected override void Attack() { }

    // ─────────────────────────────────────────
    //  Gizmos
    // ─────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, slashRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, teleportRange);
    }
}