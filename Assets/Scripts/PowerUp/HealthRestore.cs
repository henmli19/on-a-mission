using UnityEngine;

public class HealthRestore : PowerUp
{
    public float healthAmount = 2f; // zum Beispiel 2f, wuerde nach der Implementierung des Healths verandert.

    protected override void ApplyPowerUp(GameObject player)
    {
        Debug.Log($"Player restored {healthAmount} health!");

        // Es gibt kein Health im Spiel. Aber wenn es spaeter implementiert wurde, 
        // wurden der Spieler Health bekommen mit etw. wie das:
        // PlayerHealth health = player.GetComponent<PlayerHealth>();
        // if (health != null) health.Heal(health Amount);
    }
}