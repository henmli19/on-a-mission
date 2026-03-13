using Player_Scripts;
using UnityEngine;

public class Shooting : MonoBehaviour
{
  
    [SerializeField] private AudioClip laserSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Settings")]
    [SerializeField] private float shootDelay = 0.1f;
    [SerializeField] private float laserSpeed = 30f;

    private float lastShootTime;

    // Reference to PlayerMovement
    [SerializeField] private PlayerMovement playerMovement;

    void Update()
    {
        // Stop shooting if movement/input is disabled
        if (playerMovement != null && !playerMovement.inputEnabled) return;

        if (Input.GetMouseButton(0) && Time.time >= lastShootTime + shootDelay)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }

    void Shoot()
    {
        GameObject laser = Instantiate(laserPrefab, firePoint.position, Quaternion.identity);

        // Find direction
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - firePoint.position).normalized;

        // Rotate laser
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        laser.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Move laser
        Rigidbody2D rb = laser.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * laserSpeed;
        }
        audioSource.PlayOneShot(laserSound); // ADD THIS
    }
}