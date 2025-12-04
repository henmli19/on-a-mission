using UnityEngine;

public class Shield : PowerUp
{
    [Header("Shield Visual")]
    [SerializeField] private GameObject shieldVisualPrefab; // Prefab for the shield effect

    protected override void ApplyPowerUp(GameObject player)
    {
        Debug.Log($"Shield activated for {duration} seconds!");

        if (shieldVisualPrefab != null)
        {
            // Instantiate the shield visual as a child of the player
            GameObject shieldInstance = Instantiate(shieldVisualPrefab, player.transform);
            
            // Reset local position and scale so it doesn’t inherit player’s scaling
            shieldInstance.transform.localPosition = Vector3.zero;
            shieldInstance.transform.localRotation = Quaternion.identity;
            shieldInstance.transform.localScale = Vector3.one; // <-- fix: keeps its normal size

            // Destroy after duration
            Destroy(shieldInstance, duration);
        }
        // Shield funktioniert noch nicht da es kein funktionierendes Player Health und Attacks gibt.
        // PlayerHealth health = player.GetComponent<PlayerHealth>();
        // if (health != null) health.StartCoroutine(health.ApplyShield(duration));
    }
}



