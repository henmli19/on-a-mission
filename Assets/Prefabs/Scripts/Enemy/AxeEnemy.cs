using System.Collections;
using UnityEngine;

public class AxeEnemy : Enemy
{
    [Header("Movement Settings")]
    public float directionChangeDelay = 0.5f;
    public float chaseRange = 6f;  // how close the player must be for it to start chasing

    [Header("Axe Settings")]
    public Transform axeTransform;       // The axe object on the enemy’s head
    public float axeSwingAngle = 90f;    // How far the axe swings down
    public float axeSwingSpeed = 5f;     // Speed of the swing
    public float attackCooldown = 0.3f;    // Time between attacks

    private bool movingRight = true;
    private bool isAttacking = false;
    private float nextAttackTime = 0f;


    protected override void Start()
    {
        base.Start();
        moveSpeed = 1.5f;
        attackRange = 2f;

        // Find player if not set
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj) player = playerObj.transform;
        }
    }

    protected override void Update()
    {
        if (isDead) return;

        float distanceToPlayer = player ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;

        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            StartCoroutine(SwingAxe());
            nextAttackTime = Time.time + attackCooldown;
        }
        else if (!isAttacking)
        {
            // If the player is close, chase them
            if (distanceToPlayer <= chaseRange)
            {
                ChasePlayer();
            }
            else
            {
                Move();
            }
        }
    }

    private void Move()
    {
        float direction = movingRight ? 1 : -1;
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);
    }

    protected override void ChasePlayer()
    {
        if (player == null) return;

        // Move toward player
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

        // Flip to face the player
        if ((player.position.x > transform.position.x && !movingRight) ||
            (player.position.x < transform.position.x && movingRight))
        {
            Flip();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            StartCoroutine(ChangeDirection());
        }
    }

    private IEnumerator ChangeDirection()
    {
        moveSpeed = 0;
        yield return new WaitForSeconds(directionChangeDelay);
        Flip();
        moveSpeed = 1.5f;
    }

    private void Flip()
    {
        movingRight = !movingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private IEnumerator SwingAxe()
    {
        if (axeTransform == null) yield break;
        isAttacking = true;

        // Damage if player still nearby
        if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            Debug.Log($"{gameObject.name} hit the player with the axe!");
            // player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
        
        isAttacking = false;
    }

    protected override void Attack()
    {
        
    }
}

