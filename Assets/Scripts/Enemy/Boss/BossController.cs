using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    private int currentHealth;
    public float timeBetweenAttacks = 6f;
    
    [Header("References")]
    public Slider healthBar;
    public Transform player;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Slam Attack Settings")]
    public float slamDamageRadius = 3f;
    public int slamDamage = 2;
    public Transform bossSprite; // Link the visual part of the boss here

    private bool isDead = false;
    private bool isAttacking = false;
    private Vector3 centerStage;

    void Start()
    {
        currentHealth = maxHealth;
        centerStage = transform.position; // Boss remembers his starting spot

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        // Start the continuous attack loop
        StartCoroutine(BossBehaviorLoop());
    }

    IEnumerator BossBehaviorLoop()
    {
        // Wait a couple of seconds before the first attack begins
        yield return new WaitForSeconds(2f);

        while (!isDead)
        {
            if (!isAttacking)
            {
                // Pick a random number between 0, 1, and 2
                int randomAttack = Random.Range(0, 3);

                if (randomAttack == 0) yield return StartCoroutine(JumpSlamAttack());
                else if (randomAttack == 1) yield return StartCoroutine(BulletHellCone());
                else if (randomAttack == 2) yield return StartCoroutine(RadialBurstAttack());
            }

            // The 6-second cooldown between attack patterns
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    // ================== ATTACK 1: JUMP SLAM ==================
    IEnumerator JumpSlamAttack()
    {
        isAttacking = true;
        
        // 1. Jump up (Move sprite up fast)
        Vector3 startPos = transform.position;
        Vector3 highPos = startPos + new Vector3(0, 15f, 0);
        
        float t = 0;
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(startPos, highPos, t);
            t += Time.deltaTime * 3f;
            yield return null;
        }

        // 2. Hover and track player's position
        yield return new WaitForSeconds(1f);
        Vector3 targetPos = new Vector3(player.position.x, player.position.y + 1f, 0);
        transform.position = new Vector3(targetPos.x, highPos.y, 0);

        // 3. SLAM DOWN
        t = 0;
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(new Vector3(targetPos.x, highPos.y, 0), targetPos, t);
            t += Time.deltaTime * 6f; // Faster coming down
            yield return null;
        }

        // 4. Damage Check on impact
        Collider2D hit = Physics2D.OverlapCircle(transform.position, slamDamageRadius);
        if (hit != null && hit.CompareTag("Player"))
        {
            hit.GetComponent<BatteryHealthUI>()?.TakeDamage(slamDamage);
        }

        // 5. Rest a moment, then return to center
        yield return new WaitForSeconds(1.5f);
        ReturnToCenter();
        isAttacking = false;
    }

    // ================== ATTACK 2: BULLET HELL CONE ==================
    IEnumerator BulletHellCone()
    {
        isAttacking = true;
        
        // Fire 5 waves of bullets
        for (int wave = 0; wave < 5; wave++)
        {
            // Aim at player
            Vector2 dirToPlayer = (player.position - firePoint.position).normalized;
            float baseAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;

            // Fire 3 bullets in a spread per wave
            float[] angleOffsets = { -15f, 0f, 15f }; 

            foreach (float offset in angleOffsets)
            {
                GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0, 0, baseAngle + offset));
            }

            yield return new WaitForSeconds(0.4f); // Time between waves
        }

        isAttacking = false;
    }

    // ================== ATTACK 3: RADIAL BURST ==================
    IEnumerator RadialBurstAttack()
    {
        isAttacking = true;

        // Fire 3 massive 360-degree rings
        for (int wave = 0; wave < 3; wave++)
        {
            int projectilesInRing = 12; // 12 bullets per ring
            float angleStep = 360f / projectilesInRing;
            float currentAngle = 0f;

            for (int i = 0; i < projectilesInRing; i++)
            {
                // Offset every other wave slightly so they create a cool pattern
                float waveOffset = (wave % 2 == 0) ? 0 : (angleStep / 2);
                
                Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0, 0, currentAngle + waveOffset));
                currentAngle += angleStep;
            }

            yield return new WaitForSeconds(0.8f); // Wait before next ring
        }

        isAttacking = false;
    }

    // ================== UTILITIES ==================
    void ReturnToCenter()
    {
        transform.position = centerStage;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (healthBar != null) healthBar.value = currentHealth;

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            StopAllCoroutines();
            Debug.Log("Boss Defeated!");
            Destroy(gameObject, 0.5f); // Add cool explosion effect here later!
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draws a red circle in the editor so you can see the slam radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, slamDamageRadius);
    }
}