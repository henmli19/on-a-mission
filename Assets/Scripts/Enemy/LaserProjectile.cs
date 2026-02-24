using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;

    public float lifetime = 3f;
    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;

        // Calculate the angle toward the player
        // Mathf.Atan2 returns the angle in radians, Rad2Deg converts it to 0-360 degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply the rotation to the laser
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        transform.Translate(direction * (speed * Time.deltaTime), Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BatteryHealthUI health = other.GetComponent<BatteryHealthUI>();
            Debug.Log("sdfg");

            if (health != null)
            {
                health.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}