using UnityEngine;

[CreateAssetMenu(fileName = "EnergyCoreItem", menuName = "Inventory/EnergyCore")]
public class EnergyCoreItem : ScriptableObject
{
    public string itemName = "Energy Core";
    public Sprite itemSprite;
    public string itemDescription = "A glowing energy core. Someone might need this.";
}