using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Settings")]
    [SerializeField] private float shootDelay = 0.1f;
    [SerializeField] private float laserSpeed = 30f; // adjustable speed

    private float lastShootTime;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= lastShootTime + shootDelay)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }

    void Shoot()
    {
        // Spawn the laser
        GameObject laser = Instantiate(laserPrefab, firePoint.position, Quaternion.identity);

        // Find the direction from firePoint to mouse position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - firePoint.position).normalized;

        // Apply velocity to the laser
        Rigidbody2D rb = laser.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * laserSpeed;
        }

        // Flip the laser’s x-scale based on direction (optional)
        Vector3 scale = laser.transform.localScale;
        scale.x = direction.x >= 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        laser.transform.localScale = scale;
    }
}