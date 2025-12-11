using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBoostItem", menuName = "Inventory/SpeedBoost")]
public class SpeedBoostItem : ScriptableObject
{
    public string itemName = "Speed Boost";
    public Sprite itemSprite;
    public string itemDescription = "Use this item to double your speed for 5 seconds. This effect can be stacked by multiple uses at once.";
    public float speedMultiplier = 2f;
    public float duration = 5f;
}