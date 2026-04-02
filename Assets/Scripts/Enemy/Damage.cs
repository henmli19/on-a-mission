using UnityEngine;
using System.Collections;
using Player_Scripts;

public class PlayerCollision : MonoBehaviour
{
    private BatteryHealthUI healthUI;
    private PlayerMovement playerMovement;
    private Coroutine damageCoroutine;

    public float damageInterval = 1f;

    private void Start()
    {
        healthUI = GetComponent<BatteryHealthUI>();
        playerMovement = GetComponent<PlayerMovement>();

        if (healthUI == null)
            Debug.LogError("BatteryHealthUI component not found on player!");
        if (playerMovement == null)
            Debug.LogError("PlayerMovement component not found on player!");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(DamageOverTime());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private IEnumerator DamageOverTime()
    {
        while (true)
        {
            // Check shield before dealing damage
            if (playerMovement == null || !playerMovement.isShielded)
                healthUI.TakeDamage(1);

            yield return new WaitForSeconds(damageInterval);
        }
    }
}