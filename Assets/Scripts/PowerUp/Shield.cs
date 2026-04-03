using UnityEngine;

public class Shield : MonoBehaviour
{
    public ShieldItem shieldItem;
    public int quantity = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager im = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();

            int leftover = im.AddItem(
                shieldItem.itemName,
                quantity,
                shieldItem.itemSprite,
                shieldItem.itemDescription
            );

            if (leftover <= 0)
                Destroy(gameObject);
            else
                quantity = leftover; // keep remaining
        }
    }
}


