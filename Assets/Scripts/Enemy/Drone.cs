using System.Collections;
using UnityEngine;

public class Drone : Enemy
{
    [Header("Drone Settings")]
    public float hoverHeight = 1.5f;        // Fixed hover height
    public float hoverSpeed = 2f;           // Up/down motion speed
    public float hoverAmplitude = 0.1f;     // Up/down motion amplitude

    [Header("Movement")]
    public float moveRadius = 3f;           // Random roaming radius
    public float moveInterval = 3f;         // Time between roaming
    public float diveSpeed = 8f;            // Speed when diving toward player
    public float returnSpeed = 5f;          // Speed to return to hover height

    [Header("Attack")]
    public float attackDelay = 2f;          // Time between hits
    private float nextAttackTime;

    private Vector2 moveTarget;
    private float nextMoveTime;

    private enum DroneState { Roaming, Diving, Returning }
    private DroneState currentState = DroneState.Roaming;

    private Vector2 attackTarget;           // Where to dive toward (player position)
    private Vector3 hoverTarget;            // Position to return to after attack

    protected override void Start()
    {
        base.Start();
        moveSpeed = 2f;
        moveTarget = startPosition;
        hoverTarget = new Vector3(startPosition.x, startPosition.y + hoverHeight, transform.position.z);
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

        switch (currentState)
        {
            case DroneState.Roaming:
                // Apply hover sine motion
                float hoverY = Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
                Roam(hoverY);

                // Detect player
                if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRange)
                {
                    currentState = DroneState.Diving;
                    attackTarget = new Vector2(player.position.x, player.position.y);
                }
                break;

            case DroneState.Diving:
                DiveTowardPlayer();
                break;

            case DroneState.Returning:
                ReturnToHover(); // smooth return, no hover sine
                break;
        }
    }


    private void Roam(float hoverY)
    {
        // Move toward current roaming target
        transform.position = Vector2.MoveTowards(
            transform.position,
            moveTarget,
            moveSpeed * Time.deltaTime
        );

        // Pick new target after interval
        if (Time.time >= nextMoveTime)
        {
            ChooseNewTarget();
            nextMoveTime = Time.time + moveInterval;
        }

        // Keep drone at hoverHeight
        transform.position = new Vector3(
            transform.position.x,
            startPosition.y + hoverHeight + hoverY,
            transform.position.z
        );
    }

    private void DiveTowardPlayer()
    {
        if (player == null)
        {
            currentState = DroneState.Returning;
            return;
        }

        // Move fast toward the player's position
        transform.position = Vector2.MoveTowards(
            transform.position,
            attackTarget,
            diveSpeed * Time.deltaTime
        );

        // Check if reached player (within attack range)
        if (Vector2.Distance(transform.position, attackTarget) <= 0.2f)
        {
            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackDelay;
            }

            // After hitting, start returning
            currentState = DroneState.Returning;
        }
    }

    private void ReturnToHover()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            hoverTarget,
            returnSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, hoverTarget) < 0.1f)
        {
            currentState = DroneState.Roaming;
            moveTarget = startPosition;
            nextMoveTime = Time.time + moveInterval;
        }
    }

    

    private void ChooseNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * moveRadius;
        moveTarget = startPosition + randomOffset;
    }

    protected override void Attack()
    {
        // Handled by DiveTowardPlayer();
    }
}
