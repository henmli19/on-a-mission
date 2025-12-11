using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    private BatteryHealthUI healthUI;
    private Coroutine damageCoroutine;

    public float damageInterval = 1f; // damage every 1 second

    private void Start()
    {
        healthUI = GetComponent<BatteryHealthUI>();
        if (healthUI == null)
            Debug.LogError("BatteryHealthUI component not found on player!");
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
            healthUI.TakeDamage(1);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}