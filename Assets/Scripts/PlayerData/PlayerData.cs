using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySaveData {
    public string itemName;
    public int quantity;
}

[System.Serializable]
public class PlayerData {
    public int health;
    public float[] position;
    public List<InventorySaveData> inventory;

    public PlayerData(Player player, InventoryManager inv)
    {
        health = player.health;
        position = new float[] { player.transform.position.x, player.transform.position.y, player.transform.position.z };

        inventory = new List<InventorySaveData>();
        
        // Gehen durch die Slots der Inv 
        foreach (var slot in inv.itemSlot)
        {
            if (slot.quantity > 0)
            {
                inventory.Add(new InventorySaveData {
                    itemName = slot.itemName,
                    quantity = slot.quantity
                });
            }
        }
    }
}
