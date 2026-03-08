using UnityEngine;

public class HealthRestore : MonoBehaviour
{
    public HealItem healItem;
    public int quantity = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager im = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();

            int leftover = im.AddItem(
                healItem.itemName,
                quantity,
                healItem.itemSprite,
                healItem.itemDescription
            );

            if (leftover <= 0)
                Destroy(gameObject);
            else
                quantity = leftover; // keep remaining
        }
    }
}