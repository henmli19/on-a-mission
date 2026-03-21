using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 1;
    public float lifetime = 4f;

    void Start()
    {
        // Laser zustoren nach ein paar sekunden
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move forward constantly. The Boss will handle the rotation before firing.
        transform.Translate(Vector2.right * (speed * Time.deltaTime), Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Try to find the player's health script and deal damage
            BatteryHealthUI health = other.GetComponent<BatteryHealthUI>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject); 
        }
    }
}