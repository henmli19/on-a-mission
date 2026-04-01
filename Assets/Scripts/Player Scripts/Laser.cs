using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        if (collision.CompareTag("drone2"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Sphere"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }

        // Hit boss (player lasers)
        if (collision.CompareTag("Boss"))
        {
            BossEnemy boss = collision.GetComponent<BossEnemy>();
            if (boss != null)
                boss.TakeDamage(5);
            Destroy(gameObject);
        }
    }
}


