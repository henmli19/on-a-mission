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

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        transform.Translate(direction * (speed * Time.deltaTime), Space.World);
    }

    // This is what was missing — actually deal damage on hit
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BatteryHealthUI health = other.GetComponent<BatteryHealthUI>();
            if (health != null)
                health.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}