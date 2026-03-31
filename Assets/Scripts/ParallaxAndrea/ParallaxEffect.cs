using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    private Transform cam;
    private float spriteWidth;

    private GameObject[] tiles = new GameObject[3];

    private int centerIndex = 0;

    void Start()
    {
        cam = Camera.main.transform;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        // local width (important for scaled parents)
        spriteWidth = sr.sprite.bounds.size.x;

        tiles[0] = gameObject;
        tiles[1] = CreateCopy();
        tiles[2] = CreateCopy();

        // initial placement
        UpdateTiles();
    }

    GameObject CreateCopy()
    {
        GameObject copy = Instantiate(gameObject);

        copy.transform.SetParent(transform.parent);
        copy.transform.localScale = transform.localScale;

        Destroy(copy.GetComponent<ParallaxLayer>());

        return copy;
    }

    void LateUpdate()
    {
        float camX = cam.position.x * parallaxFactor;

        float localCamX = transform.parent.InverseTransformPoint(
            new Vector3(camX, 0, 0)
        ).x;

        // Handle RIGHT movement
        while (localCamX > (centerIndex + 1) * spriteWidth)
        {
            centerIndex++;
            UpdateTiles();
        }

        // Handle LEFT movement
        while (localCamX < (centerIndex - 1) * spriteWidth)
        {
            centerIndex--;
            UpdateTiles();
        }
    }

    void UpdateTiles()
    {
        for (int i = 0; i < 3; i++)
        {
            float xPos = (centerIndex + i - 1) * spriteWidth;

            tiles[i].transform.localPosition = new Vector3(
                xPos,
                transform.localPosition.y,
                transform.localPosition.z
            );
        }
    }
}