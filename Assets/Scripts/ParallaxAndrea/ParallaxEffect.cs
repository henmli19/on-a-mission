using UnityEngine;
using Cinemachine;

public class ParallaxLayer : MonoBehaviour
{
    
    [Range(0f, 1f)]
    public float parallaxFactor;

    private Transform cam;         
    private float spriteWidth;      
    private float startPosX;        

    private GameObject[] copies = new GameObject[3]; 

    void Start()
    {
        cam = Camera.main.transform;
        startPosX = transform.position.x;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        
        copies[0] = CreateCopy(-spriteWidth); 
        copies[1] = gameObject;               
        copies[2] = CreateCopy(spriteWidth);  
        
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }

    // Clones this sprite at a given horizontal offset
    GameObject CreateCopy(float offsetX)
    {
        GameObject copy = new GameObject(gameObject.name + "_copy");
        
        copy.transform.position = new Vector3(
            transform.position.x + offsetX,
            transform.position.y,
            transform.position.z
        );
        SpriteRenderer sr = copy.AddComponent<SpriteRenderer>();
        sr.sprite = GetComponent<SpriteRenderer>().sprite;
        sr.sortingLayerName = GetComponent<SpriteRenderer>().sortingLayerName;
        sr.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        copy.transform.localScale = transform.localScale;
        return copy;
    }

    // Called automatically by Cinemachine after the camera finishes moving
    void OnCameraUpdated(CinemachineBrain brain)
    {
  
        float dist = cam.position.x * parallaxFactor;
        float newX = startPosX + dist;
        
        for (int i = 0; i < 3; i++)
        {
            if (copies[i] != null)
            {
                copies[i].transform.position = new Vector3(
                    newX + (i - 1) * spriteWidth,
                    transform.position.y,
                    transform.position.z
                );
            }
        }

        // Looping logic: shift startPosX when camera moves far enough
        // This repositions the layer so it loops forever seamlessly
        float relativeCamPos = cam.position.x * (1 - parallaxFactor);
        if (relativeCamPos > startPosX + spriteWidth) startPosX += spriteWidth;
        else if (relativeCamPos < startPosX - spriteWidth) startPosX -= spriteWidth;
    }

    void OnDestroy()
    {
        // Unsubscribe from Cinemachine event when this object is destroyed
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);

        // Clean up the left and right copies we created
        if (copies[0] != null) Destroy(copies[0]);
        if (copies[2] != null) Destroy(copies[2]);
    }
}







