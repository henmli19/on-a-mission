using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public SpeedBoostItem speedBoostItem;
    public int quantity = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager im = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();

            int leftover = im.AddItem(
                speedBoostItem.itemName,
                quantity,
                speedBoostItem.itemSprite,
                speedBoostItem.itemDescription
            );

            if (leftover <= 0)
                Destroy(gameObject);
            else
                quantity = leftover; // keep remaining
        }
    }
}