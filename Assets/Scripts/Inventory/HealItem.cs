using UnityEngine;

[CreateAssetMenu(fileName = "HealItem", menuName = "Inventory/Heal")]
public class HealItem : ScriptableObject
{
    public string itemName = "Heal";
    public Sprite itemSprite;
    public string itemDescription = "Use this item to heal 1 HP.";
}