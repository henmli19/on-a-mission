using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
public class Cards_Script : MonoBehaviour {

    [SerializeField] private Image iconImage;// UI Image component displaying the sprite
    public Sprite hiddenIconSprite;// Back side of the card
    public Sprite iconSprite;// Front side sprite

    public bool isSelected;// Indicates whether card is currently revealed
    public CardController Controller;// Reference to main controller

    public void OnCardClick() {
        // Forward click event to controller (centralized logic)
        Controller.SetSelected(this);
    }

    public void SetIconSprite(Sprite sprite) {

        iconSprite = sprite;

        if (sprite != null && sprite.name == "dron_enemy")
        {
            iconImage.rectTransform.sizeDelta = new Vector2(230, 135);
        }
        else if (sprite != null && sprite.name == "AI_NPC_2")
        {
            iconImage.rectTransform.sizeDelta = new Vector2(160, 165);
        }
        else if (sprite != null && sprite.name == "Health_GameObjekt")
        {
            iconImage.rectTransform.sizeDelta = new Vector2(160, 160);
        }
        
        else if (sprite != null && sprite.name == "Rust_NPC_3")
        {
            iconImage.rectTransform.sizeDelta = new Vector2(151, 169);
        }
        
        else if (sprite != null && sprite.name == "Minimap")
        {
            iconImage.rectTransform.sizeDelta = new Vector2(180, 180);
        }
        else if (sprite != null && sprite.name == "NPC_1")
        {
            iconImage.rectTransform.sizeDelta = new Vector2(173, 152);
        }
        else
        {
            iconImage.rectTransform.sizeDelta = new Vector2(230, 200); // normal
        }
    }

    public void ShowCard() {

        // Rotate to simulate flip animation
        Tween.Rotation(transform, new Vector3(0f, 180f, 0f), 0.2f);

        // Change sprite through animation
        Tween.Delay(0.1f, () => iconImage.sprite = iconSprite);

        isSelected = true;
    }

    public void HideCard() {

        Tween.Rotation(transform, new Vector3(0f, 0f, 0f), 0.2f);

        Tween.Delay(0.1f, () => {
            iconImage.sprite = hiddenIconSprite;
            isSelected = false;
        });
    }
}
