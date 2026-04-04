using UnityEngine;
using Player_Scripts;

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

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
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
            // Check shield before dealing damage
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null && pm.isShielded)
            {
                Destroy(gameObject); // laser still disappears on hit
                return;
            }

            BatteryHealthUI health = other.GetComponent<BatteryHealthUI>();
            if (health != null)
                health.TakeDamage(damage);

            Destroy(gameObject);
        }

        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }

        if (other.CompareTag("Laser"))
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}