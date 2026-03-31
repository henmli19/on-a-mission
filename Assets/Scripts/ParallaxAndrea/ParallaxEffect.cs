using UnityEngine;

public class ParallaxLayerClean : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    private Transform cam;
    private float spriteWidth;

    private GameObject[] tiles = new GameObject[3];

    void Start()
    {
        cam = Camera.main.transform;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        // 🔥 IMPORTANT: use LOCAL width (ignores parent scaling issues)
        spriteWidth = sr.sprite.bounds.size.x;

        tiles[0] = gameObject;
        tiles[1] = CreateCopy();
        tiles[2] = CreateCopy();
    }

    GameObject CreateCopy()
    {
        GameObject copy = Instantiate(gameObject);

        copy.transform.SetParent(transform.parent);

        // keep EXACT same local scale
        copy.transform.localScale = transform.localScale;

        Destroy(copy.GetComponent<ParallaxLayerClean>());

        return copy;
    }

    void LateUpdate()
    {
        float camX = cam.position.x * parallaxFactor;

        // convert camera position into LOCAL space of parent
        float localCamX = transform.parent.InverseTransformPoint(new Vector3(camX, 0, 0)).x;

        int centerIndex = Mathf.RoundToInt(localCamX / spriteWidth);

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





