using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    // Stats
    public float maxHealth = 100f;
    protected float currentHealth;

    public float moveSpeed;
    public float damage = 10f;

    // Detection
    public float detectionRange = 10f;   // Spieler wird in diesem Radius erkannt
    public float attackRange = 10f;    // Reichweite für Angriff
    public LayerMask playerLayer;

    // Patrol
    public float patrolRadius = 3f;     // Bereich um den Spawn, in dem der Gegner patrouilliert
    protected Vector2 startPosition;
    protected Vector2 patrolTarget;

    protected Transform player;
    protected bool isDead = false;
    
    public static QuestKillEnemies quest;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;
        ChooseNewPatrolTarget();
    }

    protected virtual void Update()
    {
        if (isDead) return;

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

    // --- Grundverhalten ---
    protected virtual void Patrol()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            patrolTarget,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f)
        {
            ChooseNewPatrolTarget();
        }
    }

    protected void ChooseNewPatrolTarget()
    {
        Vector2 randomPoint = startPosition + Random.insideUnitCircle * patrolRadius;
        patrolTarget = randomPoint;
    }

    protected virtual void ChasePlayer()
    {
        if (player == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );
    }

    // --- Kampf & Schaden ---
    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    protected virtual void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " is dead!");
        Destroy(gameObject, 1f); // kurz Delay für Animation
        
        quest.EnemyKilled();
    }

    protected abstract void Attack(); // wird in Kindklassen implementiert
}
