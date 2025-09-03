using UnityEngine;

public class WinManager : MonoBehaviour
{
    public GameObject[] boxes;           // Assign your 2 boxes here
    public GameObject textCollect;       // First text (e.g. "Collect the boxes")
    public GameObject textWin;           // Second text (e.g. "You won!")

    private int collectedCount = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we touched one of the boxes
        for (int i = 0; i < boxes.Length; i++)
        {
            if (other.gameObject == boxes[i])
            {
                boxes[i].SetActive(false);   // Hide that box
                collectedCount++;            // Count it
            }
        }

        // If all boxes are collected → show win text
        if (collectedCount >= boxes.Length)
        {
            textCollect.SetActive(false); // Hide "collect" text
            textWin.SetActive(true);      // Show "You Won!"
            Time.timeScale = 0;           // Optional: pause game
        }
    }
}