using System.Collections;
using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
    public Transform groundMoving;
    public float moveDistance = 1.4f; 
    public float moveTime = 0.5f;      

    private bool hasTriggered = false; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; 
            StartCoroutine(MoveGroundSmoothly());
        }
    }

    private IEnumerator MoveGroundSmoothly()
    {
        Vector3 startPos = groundMoving.position;
        Vector3 endPos = new Vector3(startPos.x + moveDistance, startPos.y, startPos.z);

        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            groundMoving.position = Vector3.Lerp(startPos, endPos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        groundMoving.position = endPos; 
    }
}