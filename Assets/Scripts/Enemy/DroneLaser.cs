using System.Collections;
using UnityEngine;

public class DroneLaser : Enemy
{
    [Header("Drone Settings")]
    public float hoverHeight = 1.5f;
    public float hoverSpeed = 2f;
    public float hoverAmplitude = 0.1f;

    [Header("Movement")]
    public float moveRadius = 3f;
    public float moveInterval = 3f;

    [Header("Attack")]
    public float attackInterval = 2f;
    public GameObject laserPrefab;
    public Transform firePoint;
    public float laserSpeed = 10f;

    private float nextAttackTime;
    private Vector2 moveTarget;
    private float nextMoveTime;
    private bool isAttacking = false;

    protected override void Start()
    {
        base.Start();
        moveSpeed = 2.5f;
        moveTarget = (Vector2)transform.position; // Set initial target to current spot
        nextMoveTime = Time.time + moveInterval;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj) player = playerObj.transform;
        }
    }

    protected override void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 1. Detection Logic
        isAttacking = distanceToPlayer <= detectionRange;

        if (isAttacking)
        {
            // 2. Attack Logic
            if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackInterval;
            }
            // Note: Rotation code is GONE. He stays facing the same way.
        }
        else
        {
            // 3. Only Patrol if NOT attacking
            Patrol();
        }

        // 4. Hover Logic (Applied to the visual offset, not hard-resetting position)
        float hoverY = Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + (hoverY * Time.deltaTime * 10), // Subtle hover adjustment
            transform.position.z
        );
    }

    protected override void Attack()
    {
        if (laserPrefab == null || firePoint == null) return;

        Vector2 direction = (player.position - firePoint.position).normalized;

        // We spawn with identity, but SetDirection will immediately fix the rotation
        GameObject laser = Instantiate(laserPrefab, firePoint.position, Quaternion.identity);
       
        LaserProjectile laserScript = laser.GetComponent<LaserProjectile>();
        if (laserScript != null)
        {
            laserScript.SetDirection(direction, laserSpeed);
        }
    }

    protected override void Patrol()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            moveTarget,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, moveTarget) < 0.2f || Time.time >= nextMoveTime)
        {
            ChooseNewTarget();
            nextMoveTime = Time.time + moveInterval;
        }
    }

    private void ChooseNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * moveRadius;
        moveTarget = (Vector2)startPosition + randomOffset;
    }
}