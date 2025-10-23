using UnityEngine;

public class Shield : PowerUp
{
    protected override void ApplyPowerUp(GameObject player)
    {
        Debug.Log($"Shield activated for {duration} seconds!");

        // Shield funktioniert noch nicht da es kein funktionierendes Player Health und Attacks gibt.
        // PlayerHealth health = player.GetComponent<PlayerHealth>();
        // if (health != null) health.StartCoroutine(health.ApplyShield(duration));
    }
}


