using UnityEngine;

public class Target : MonoBehaviour {
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ParticleSystem explosionEffect;

    private void OnMouseDown() {
        // Show the target effect.
        explosionEffect.transform.position = transform.position;
        explosionEffect.Play();

        // Devolve what to do to the GameManager.
        gameManager.TargetHit();
    }
}