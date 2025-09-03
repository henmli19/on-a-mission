using UnityEngine;

public class RedWin : MonoBehaviour
{
    public GameObject textBoxToDeactivate;
    public GameObject textBoxToActivate;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (textBoxToDeactivate != null)
                textBoxToDeactivate.SetActive(false);

            if (textBoxToActivate != null)
                textBoxToActivate.SetActive(true);
        }
    }
}