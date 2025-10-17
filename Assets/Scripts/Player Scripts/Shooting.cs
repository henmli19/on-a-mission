using UnityEngine;

public class RobotShooting : MonoBehaviour
{
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform firePoint;
    
    [SerializeField] private float shootDelay = 0.1f; // seconds between shots
    private float lastShootTime = 0f;

    void Update()
    {
        // Check if enough time has passed since the last shot
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastShootTime + shootDelay)
        {
            Shoot();
            lastShootTime = Time.time; // reset timer
        }
    }

    void Shoot()
    {
        GameObject laser = Instantiate(laserPrefab, firePoint.position, Quaternion.identity);

        bool facingRight = transform.localScale.x > 0;

        Rigidbody2D rb = laser.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(facingRight ? 10f : -10f, 0f); 
        }

        Vector3 scale = laser.transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        laser.transform.localScale = scale;
    }
}