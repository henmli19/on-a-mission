using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    [Header("PowerUp Settings")]
    public float duration = 5f; // for temporary effects (like speed/shield)

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyPowerUp(other.gameObject);
            Destroy(gameObject);
        }
    }

    protected abstract void ApplyPowerUp(GameObject player);
}