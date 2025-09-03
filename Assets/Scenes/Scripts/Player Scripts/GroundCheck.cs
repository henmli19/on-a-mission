using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [HideInInspector] public bool isGrounded;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))   // only platforms tagged as "Ground"
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}