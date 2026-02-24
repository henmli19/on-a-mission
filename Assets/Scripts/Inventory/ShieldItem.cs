using UnityEngine;

[CreateAssetMenu(fileName = "ShieldItem", menuName = "Inventory/Shield")]
public class ShieldItem : ScriptableObject
{
    public string itemName = "Shield";
    public Sprite itemSprite;
    public string itemDescription = "Use this item to shield yourself for 5 seconds.";
    public float duration = 5f;
}