using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool menuActivated;
    public ItemSlot[] itemSlot;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory") && menuActivated)
        {
            Time.timeScale = 1;
            InventoryMenu.SetActive(false); // Hide Inventory
            menuActivated = false;
        }
        else if (Input.GetButtonDown("Inventory") && !menuActivated)
        {
            Time.timeScale = 0;
            InventoryMenu.SetActive(true); // Show Inventory
            menuActivated = true;
        }
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        // Probieren, Objekte im gleichen Slot hinzufuegen
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].itemName == itemName && !itemSlot[i].isFull)
            {
                int leftOverItems = itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                if (leftOverItems <= 0) return 0;

                quantity = leftOverItems;
            }
        }

        // Andere items in empty slot
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].quantity == 0)
            {
                int leftOverItems = itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                return leftOverItems;
            }
        }
        return quantity;
    }
    
    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].thisItemSelected = false;
        }
    }
    
    public void LoadInventory(List<InventorySaveData> savedItems)
    {
        // Clear all current slots first
        foreach (var slot in itemSlot)
        {
            slot.ClearSlot();
        }

        // Fill slots from save data
        foreach (var data in savedItems)
        {
            // Look for the ScriptableObject in Resources/Items/
            SpeedBoostItem itemData = Resources.Load<SpeedBoostItem>("Items/" + data.itemName);

            if (itemData != null)
            {
                AddItem(itemData.itemName, data.quantity, itemData.itemSprite, itemData.itemDescription);
            }
            else
            {
                Debug.LogWarning("Could not find Item Data for: " + data.itemName);
            }
        }
    }
}
