// NPCInteraction.cs
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("Einstellungen")]
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;
    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        QuestManager.Instance.StartEnergyQuest();
    }

    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        playerInRange = distance <= interactionRadius;

        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            Interact();
        }
    }

    private void Interact()
    {
        if (QuestManager.Instance.energyCoreQuestState == QuestManager.QuestState.NotStarted)
            return;

        if (QuestManager.Instance.energyCoreQuestState == QuestManager.QuestState.Completed)
            return;

        ItemSlot energyCoreSlot = FindEnergyCore();

        if (energyCoreSlot != null)
        {
            energyCoreSlot.quantity--;
            if (energyCoreSlot.quantity <= 0)
                energyCoreSlot.ClearSlot();

            QuestManager.Instance.CompleteEnergyQuest();
        }
    }

    private ItemSlot FindEnergyCore()
    {
        foreach (ItemSlot slot in inventoryManager.itemSlot)
        {
            if (slot.itemName == "Energy Core" && slot.quantity > 0)
                return slot;
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}