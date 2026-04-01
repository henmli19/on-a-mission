using UnityEngine;

public class EnergyItem : MonoBehaviour
{
    public EnergyCoreItem energyCoreItem;
    public int quantity = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager im = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();

            int leftover = im.AddItem(
                energyCoreItem.itemName,
                quantity,
                energyCoreItem.itemSprite,
                energyCoreItem.itemDescription
            );

            if (leftover <= 0)
                Destroy(gameObject);
            else
                quantity = leftover;
        }
    }
}