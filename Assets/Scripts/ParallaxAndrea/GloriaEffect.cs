using UnityEngine;
 
public class ParallaxEffect : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float parallaxMultiplier = 0.5f;
 
    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        transform.position += Vector3.left * (moveInput * moveSpeed * parallaxMultiplier * Time.deltaTime);
    }
}