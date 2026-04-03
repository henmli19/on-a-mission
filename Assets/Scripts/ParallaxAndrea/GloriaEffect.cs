using UnityEngine;
using Cinemachine;

public class ParallaxLayer2 : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxFactor;

    private Transform cam;
    private float startPosX;

    void Start()
    {
        cam = Camera.main.transform;
        startPosX = transform.position.x;

        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }

    void OnCameraUpdated(CinemachineBrain brain)
    {
        float dist = cam.position.x * parallaxFactor;
        float newX = startPosX + dist;

        transform.position = new Vector3(
            newX,
            transform.position.y,
            transform.position.z
        );
    }

    void OnDestroy()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }
}