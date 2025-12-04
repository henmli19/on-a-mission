using System.Collections;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float moveDistance = 5f;       // How far the elevator moves
    public float moveSpeed = 2f;          // Elevator speed
    private bool isMoving = false;        // Prevent multiple moves
    private bool playerOnElevator = false;

    private Vector3 startPos;
    private Vector3 targetPos;

    private void Start()
    {
        startPos = transform.position;    // Save starting position
        targetPos = startPos;             // Initialize target
    }

    private void Update()
    {
        // Check if player is on elevator and presses E
        if (playerOnElevator && Input.GetKeyDown(KeyCode.E) && !isMoving)
        {
            isMoving = true;

            // Decide direction based on current position
            if (transform.position == startPos)
            {
                targetPos = startPos + Vector3.up * moveDistance; // Move up
            }
            else
            {
                targetPos = startPos; // Move back down
            }

            StartCoroutine(MoveElevator());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnElevator = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnElevator = false;
        }
    }

    private IEnumerator MoveElevator()
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }
}