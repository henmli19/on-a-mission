using UnityEngine;

public class Item : MonoBehaviour
{

    [SerializeField] private string itemName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private int quantity;
    [TextArea] [SerializeField] private string itemDescription;
   
    [SerializeField] private AudioSource audioSource;
    private InventoryManager inventoryManager;
    
    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
            
        {
           
            int leftOverItems = inventoryManager.AddItem(itemName, quantity, sprite, itemDescription);
            if (leftOverItems <= 0) Destroy(gameObject);
            else quantity = leftOverItems;
        }
    }
}
