using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    public float lifetime = 3f;
    public float damage = 10f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Example damage logic (depends on your player script)
            Debug.Log("Laser hit the player!");
            // You can call: other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}