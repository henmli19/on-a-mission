using UnityEngine;

public class MinigameStart : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject barrier;

    private bool hasStarted = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasStarted && other.CompareTag("Player"))
        {
            hasStarted = true;
            gameManager.StartGame(other.gameObject, barrier);
        }
    }
}