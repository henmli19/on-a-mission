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
    private PlayerMovement pm;
    
    void Start()
    {
        // This is where you assign the component!
        pm = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // 1. Check if we have the reference
        if (pm == null) return;

        // 2. STOP SHOOTING if grabbed OR if input is disabled
        // We use the helper function from PlayerMovement
        if (pm.IsBeingGrabbed() || !pm.inputEnabled) return;

        if (Input.GetMouseButton(0) && Time.time >= lastShootTime + shootDelay)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }

    void Shoot()
    {
        if (laserPrefab == null || firePoint == null) return;

        GameObject laser = Instantiate(laserPrefab, firePoint.position, Quaternion.identity);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - firePoint.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        laser.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rb = laser.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * laserSpeed;
        }
        audioSource.PlayOneShot(laserSound); // ADD THIS
    }
}