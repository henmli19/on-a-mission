using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxFactor;

    private Transform cam;
    private Vector3 startPos;

    void Start()
    {
        cam = Camera.main.transform;
        startPos = transform.position;
    }

    void Update()
    {
        float offsetX = cam.position.x * parallaxFactor;
        transform.position = new Vector3(
            startPos.x + offsetX,
            startPos.y,
            startPos.z
        );
    }
}