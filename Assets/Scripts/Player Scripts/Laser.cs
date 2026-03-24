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
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);          
        }
    }
}


