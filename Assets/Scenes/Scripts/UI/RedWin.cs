using UnityEngine;

public class RedWin : MonoBehaviour
{
    public GameObject textBoxToDeactivate;
    public GameObject textBoxToActivate;
    public GameObject boxToDeactivate;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (textBoxToDeactivate != null)
                textBoxToDeactivate.SetActive(false);

            if (textBoxToActivate != null)
                textBoxToActivate.SetActive(true);
            if (boxToDeactivate != null)
                boxToDeactivate.SetActive(false);
            Time.timeScale = 0;
        }
    }
}