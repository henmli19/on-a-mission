using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
{
    public int health = 100;
    public InventoryManager inventory; // Drag your InventoryCanvas here in Inspector

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this, inventory);
        Debug.Log("Game Saved!");
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        if (data != null)
        {
            // 1. Restore Stats
            this.health = data.health;

            // 2. Restore Position
            Vector3 pos;
            pos.x = data.position[0];
            pos.y = data.position[1];
            pos.z = data.position[2];
            transform.position = pos;

            // 3. Restore Inventory
            inventory.LoadInventory(data.inventory);
            
            Debug.Log("Game Loaded!");
        }
    }
}