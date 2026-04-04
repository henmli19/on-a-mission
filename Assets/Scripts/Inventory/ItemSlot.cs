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

    public PowerUpTimerUI speedTimer;
    public PowerUpTimerUI shieldTimer;

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    // ─────────────────────────────────────────
    //  Add Item
    // ─────────────────────────────────────────
    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        if (isFull) return quantity;

        this.itemName        = itemName;
        this.itemSprite      = itemSprite;
        this.itemDescription = itemDescription;
        itemImage.sprite     = itemSprite;

        this.quantity += quantity;

        if (this.quantity >= maxNumberOfItems)
        {
            quantityText.text    = maxNumberOfItems.ToString();
            quantityText.enabled = true;
            isFull               = true;

            int extraItems = this.quantity - maxNumberOfItems;
            this.quantity  = maxNumberOfItems;
            return extraItems;
        }

        quantityText.text    = this.quantity.ToString();
        quantityText.enabled = true;
        return 0;
    }

    // ─────────────────────────────────────────
    //  Click Handling
    // ─────────────────────────────────────────
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            OnLeftClick();

        if (eventData.button == PointerEventData.InputButton.Right)
            OnRightClick();
    }

    public void OnLeftClick()
    {
        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        thisItemSelected             = true;
        itemDescriptionNameText.text = itemName;
        itemDescriptionText.text     = itemDescription;
        itemDescriptionImage.sprite  = itemSprite;

        if (itemDescriptionImage.sprite == null)
            itemDescriptionImage.sprite = emptySprite;
    }

    public void OnRightClick()
    {
        if (quantity <= 0) return; // nothing in slot, do nothing

        switch (itemName)
        {
            case "Speed Boost": UseSpeedBoost(); break;
            case "Shield":      UseShield();     break;
            case "Heal":        UseHeal();       break;
        }
    }

    // ─────────────────────────────────────────
    //  Use Items
    // ─────────────────────────────────────────
    private void UseSpeedBoost()
    {
        PlayerMovement robot = GameObject.FindGameObjectWithTag("Player")
                                         .GetComponent<PlayerMovement>();
        if (robot != null)
            robot.StartCoroutine(robot.ApplySpeedBoost(2f, 5f));

        ConsumeItem();

        if (PowerUpTimerUI.SpeedInstance == null)
            PowerUpTimerUI.SpeedInstance = Resources
                .FindObjectsOfTypeAll<PowerUpTimerUI>()
                .FirstOrDefault(t => !t.isShieldTimer);

        if (PowerUpTimerUI.SpeedInstance != null)
        {
            PowerUpTimerUI.SpeedInstance.gameObject.SetActive(true);
            PowerUpTimerUI.SpeedInstance.StartTimer();
        }
    }

    private void UseShield()
    {
        PlayerMovement robot = GameObject.FindGameObjectWithTag("Player")
                                         .GetComponent<PlayerMovement>();
        if (robot != null)
            robot.StartCoroutine(robot.ApplyShield(5f));

        ConsumeItem();

        if (PowerUpTimerUI.ShieldInstance == null)
            PowerUpTimerUI.ShieldInstance = Resources
                .FindObjectsOfTypeAll<PowerUpTimerUI>()
                .FirstOrDefault(t => t.isShieldTimer);

        if (PowerUpTimerUI.ShieldInstance != null)
        {
            PowerUpTimerUI.ShieldInstance.gameObject.SetActive(true);
            PowerUpTimerUI.ShieldInstance.StartTimer();
        }
    }

    private void UseHeal()
    {
        BatteryHealthUI health = GameObject.FindGameObjectWithTag("Player")
                                            .GetComponent<BatteryHealthUI>();
        if (health != null)
        {
            health.Heal(1);
            ConsumeItem();
            Debug.Log("Player has been healed!");
        }
    }

    // ─────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────

    // Reduces quantity by 1 and clears slot if empty
    private void ConsumeItem()
    {
        quantity--;

        if (quantity <= 0)
            ClearSlot();
        else
            quantityText.text = quantity.ToString();
    }

    public void ClearSlot()
    {
        itemName        = "";
        itemDescription = "";
        itemSprite      = emptySprite;
        itemImage.sprite = emptySprite;
        isFull          = false;
        quantity        = 0;
        quantityText.enabled = false;
    }
}