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
        moveTarget = startPosition;
        nextMoveTime = Time.time + moveInterval;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj) player = playerObj.transform;
        }
    }

    protected override void Update()
    {
        if (isDead) return;

        float hoverY = Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        float distanceToPlayer = player ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;

        isAttacking = distanceToPlayer <= detectionRange;

        if (isAttacking)
        {
            if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackInterval;
            }

            // Face player visually
            Vector3 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        else
        {
            Patrol();
        }

        // Hover up and down
        transform.position = new Vector3(
            transform.position.x,
            startPosition.y + hoverHeight + hoverY,
            transform.position.z
        );
    }

    protected override void Attack()
    {
        if (laserPrefab == null || firePoint == null)
        {
            Debug.LogWarning("DroneLaser missing laserPrefab or firePoint!");
            return;
        }

        // Get direction from firePoint to player
        Vector2 direction = (player.position - firePoint.position).normalized;

        // Spawn laser
        GameObject laser = Instantiate(laserPrefab, firePoint.position, Quaternion.identity);

        // Rotate laser to face the player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        laser.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Move it toward player
        LaserProjectile laserScript = laser.GetComponent<LaserProjectile>();
        if (laserScript != null)
        {
            laserScript.SetDirection(direction, laserSpeed);
        }

        Debug.Log($"{gameObject.name} fired a laser toward {player.name}");
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
        moveTarget = startPosition + randomOffset;
    }
}
