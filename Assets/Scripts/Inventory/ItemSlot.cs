using System;
using System.Linq;
using Player_Scripts;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    //========= ITEM DATA ============
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;

    [SerializeField] private int maxNumberOfItems;

    public Sprite emptySprite;

    //========= ITEM SLOT ============
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;


    //=== ITEM DESCRIPTION SLOT ===
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;

    public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        // Check if the Slot is full.
        if (isFull) return quantity;

        // Update the name
        this.itemName = itemName;

        // Update the image
        this.itemSprite = itemSprite;
        itemImage.sprite = itemSprite;

        // Update the description
        this.itemDescription = itemDescription;

        // Update the quantity
        this.quantity += quantity;

        if (this.quantity >= maxNumberOfItems)
        {
            quantityText.text = maxNumberOfItems.ToString();
            quantityText.enabled = true;
            isFull = true;

            //RETURN THE LEFTOVERS
            int extraItems = this.quantity - maxNumberOfItems;
            this.quantity = maxNumberOfItems;
            return extraItems;
        }

        // Update QUANTITY TEXT
        quantityText.text = this.quantity.ToString();
        quantityText.enabled = true;

        return 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        thisItemSelected = true;
        itemDescriptionNameText.text = itemName;
        itemDescriptionText.text = itemDescription;
        itemDescriptionImage.sprite = itemSprite;
        if (itemDescriptionImage.sprite == null) itemDescriptionImage.sprite = emptySprite;
    }

    public void OnRightClick()
    {
        if (itemName == "Speed Boost")
        {
            UseSpeedBoost();
        } else if (itemName == "Shield")
        {
            UseShield();
        }
    }
    
    public PowerUpTimerUI speedTimer;
    public PowerUpTimerUI shieldTimer;
    
    private void UseSpeedBoost()
    {
        PlayerMovement robot = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        if (robot != null)
        {
            // Beginn SpeedBoost Coroutine
            robot.StartCoroutine(robot.ApplySpeedBoost(2f, 5f));
        }

        // Quantity vom Power Ups reduzieren.
        quantity--;

        if (quantity <= 0)
        {
            ClearSlot();
        }
        else
        {
            quantityText.text = quantity.ToString();
        }
        
        if (PowerUpTimerUI.SpeedInstance == null)
        {
            // Suche nach alle Objekte aktiv und inaktiv
            PowerUpTimerUI.SpeedInstance = Resources.FindObjectsOfTypeAll<PowerUpTimerUI>().FirstOrDefault(t => !t.isShieldTimer);
        }

        if (PowerUpTimerUI.SpeedInstance != null)
        {
            PowerUpTimerUI.SpeedInstance.gameObject.SetActive(true);
            PowerUpTimerUI.SpeedInstance.StartTimer();
        }
    }
    
    private void UseShield()
    {
        PlayerMovement robot = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        if (robot != null)
        {
            // Start Coroutine
            robot.StartCoroutine(robot.ApplyShield(5f));
        }

        quantity--;

        if (quantity <= 0)
        {
            ClearSlot();
        }
        else
        {
            quantityText.text = quantity.ToString();
        }

        if (PowerUpTimerUI.ShieldInstance == null)
        {
            // Suche nach alle Objekte aktiv und inaktiv
            PowerUpTimerUI.ShieldInstance = Resources.FindObjectsOfTypeAll<PowerUpTimerUI>().FirstOrDefault(t => t.isShieldTimer);
        }

        if (PowerUpTimerUI.ShieldInstance != null)
        {
            PowerUpTimerUI.ShieldInstance.gameObject.SetActive(true);
            PowerUpTimerUI.ShieldInstance.StartTimer();
        }
    }
    
    public void ClearSlot()
        {
            itemName = "";
            itemDescription = "";
            itemSprite = emptySprite;
            itemImage.sprite = emptySprite;
            isFull = false;
            quantity = 0;
            quantityText.enabled = false;
        }
}
