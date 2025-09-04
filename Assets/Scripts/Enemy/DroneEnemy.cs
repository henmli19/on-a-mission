using UnityEngine;
using System.Collections;

public class DroneEnemy : Enemy
{
    [Header("Drone Settings")]
    public float hoverHeight = 1.5f;

    [Header("Movement Speeds")]
    public float moveSpeedGeneral = 5f;   // Normal movement
    public float moveSpeedAttack = 4f;    // Slow approach during wind-up
    public float maxChargeSpeed = 10f;    // Peak speed during charge

    [Header("Attack Settings")]
    public float chargeCooldown = 2f;
    public float windupTime = 0.6f;
    public float chargeTime = 0.8f;

    [Header("Hover Effect")]
    public float hoverAmplitude = 0.2f;   // Bob height
    public float hoverFrequency = 2f;     // Bob speed

    private bool isCharging = false;
    private bool isWindup = false;
    private float lastChargeTime = -999f;
    private float hoverOffset = 0f;

    protected override void Start()
    {
        base.Start();
        transform.position = new Vector3(transform.position.x, hoverHeight, transform.position.z);
    }

    protected override void Update()
    {
        if (isDead) return;

        if (player == null)
        {
            Collider2D playerInRange = Physics2D.OverlapCircle(
                transform.position,
                detectionRange,
                playerLayer
            );
            if (playerInRange != null)
                player = playerInRange.transform;
        }

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
            else if (distanceToPlayer <= detectionRange)
            {
                ChasePlayer();
            }
            else
            {
                Patrol();
            }
        }
        else
        {
            Patrol();
        }
    }

    protected override void Patrol()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            patrolTarget,
            moveSpeedGeneral * Time.deltaTime
        );

        ApplyHover();

        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f)
            ChooseNewPatrolTarget();
    }

    protected override void ChasePlayer()
    {
        if (!isCharging && !isWindup && player != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                moveSpeedGeneral * Time.deltaTime
            );

            ApplyHover();
        }
    }

    protected override void Attack()
    {
        if (isCharging || isWindup) return;

        if (Time.time - lastChargeTime >= chargeCooldown && player != null)
        {
            StartCoroutine(ChargeAttack());
        }
    }

    private void ApplyHover()
    {
        hoverOffset = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        transform.position = new Vector3(transform.position.x, hoverHeight + hoverOffset, transform.position.z);
    }

private IEnumerator ChargeAttack()
{
    if (player == null) yield break;

    Vector3 positionBeforeAttack = transform.position;
    Vector3 targetPosition = player.position; // lock target at start
    lastChargeTime = Time.time;

    // ----- WIND-UP -----
    isWindup = true;
    float elapsed = 0f;
    while (elapsed < windupTime)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeedAttack * Time.deltaTime
        );
        // Do NOT apply hover here; allow Y to move naturally toward player
        elapsed += Time.deltaTime;
        yield return null;
    }
    isWindup = false;

    // ----- CHARGE -----
    isCharging = true;
    elapsed = 0f;
    Quaternion originalRotation = transform.rotation;
    Quaternion chargeRotation = Quaternion.Euler(0, 0, -15f);

    while (elapsed < chargeTime)
    {
        float t = elapsed / chargeTime;
        float speedFactor = Mathf.Sin(t * Mathf.PI);
        float currentSpeed = Mathf.Lerp(moveSpeedAttack, maxChargeSpeed, speedFactor);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            currentSpeed * Time.deltaTime
        );

        // Lean forward for effect
        transform.rotation = Quaternion.Slerp(originalRotation, chargeRotation, speedFactor);

        elapsed += Time.deltaTime;
        yield return null;
    }

// ----- RETURN -----
    elapsed = 0f;
    float returnTime = 1.5f;
    Vector3 startPos = transform.position;

    while (elapsed < returnTime)
    {
        float t = elapsed / returnTime;
        float easeT = 1 - Mathf.Pow(1 - t, 3); // cubic ease-out for smooth start/stop

        transform.position = Vector3.Lerp(startPos, positionBeforeAttack, easeT);
        ApplyHover(); // hover applied during return for smooth bobbing

        // smoothly rotate back upright
        transform.rotation = Quaternion.Slerp(chargeRotation, originalRotation, easeT);

        elapsed += Time.deltaTime;
        yield return null;
    }

// Snap to exact position at the end to avoid tiny offsets
    transform.position = positionBeforeAttack;
    transform.rotation = originalRotation;
    isCharging = false;

}

}




