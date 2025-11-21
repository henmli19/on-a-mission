using UnityEngine;

public class MiniMapFollower : MonoBehaviour
{
    public Transform player;
    public float offsetZ = -10f; // set to your camera's Z

    void LateUpdate()
    {
        transform.position = new Vector3(
            player.position.x,   // follow horizontally
            player.position.y,   // follow vertically (if your level scrolls)
            offsetZ              // stay at same depth
        );
    }
}