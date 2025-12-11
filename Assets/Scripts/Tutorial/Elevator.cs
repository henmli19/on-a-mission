using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Transform pointA; // Bottom
    public Transform pointB; // Top
    public float speed = 3f;

    private bool moving = false;
    private Vector3 target;
    private bool goingUp = true; // track direction manually

    private Transform player; // player on elevator

    void Update()
    {
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                moving = false;
                goingUp = !goingUp; // reverse direction for next press
            }
        }

        if (player != null && Input.GetKeyDown(KeyCode.E) && !moving)
        {
            target = goingUp ? pointB.position : pointA.position;
            moving = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.transform;
            // Optional: carry player
            player.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.SetParent(null);
            player = null;
        }
    }
}