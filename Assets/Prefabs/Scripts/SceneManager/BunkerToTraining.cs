using UnityEngine;
using UnityEngine.SceneManagement; // Needed for scene loading

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string TrainingArc; // Name of the scene to load

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Only triggers with the player
        {
            SceneManager.LoadScene(TrainingArc);
            
        }
    }
}