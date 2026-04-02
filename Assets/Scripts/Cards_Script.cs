using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class Cards_Script : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    public Sprite hiddenIconSprite;
    public Sprite iconSprite;

    public bool isSelected;
    public CardController Controller;

    public void OnCardClick()
    {
        Controller.SetSelected(this);
    }

    public void SetIconSprite(Sprite sprite)
    {
        iconSprite = sprite;

        if (sprite == null) return;

        switch (sprite.name)
        {
            case "dron_enemy":
                iconImage.rectTransform.sizeDelta = new Vector2(230, 135);
                break;
            case "AI_NPC_2":
                iconImage.rectTransform.sizeDelta = new Vector2(160, 165);
                break;
            case "Health_GameObjekt":
                iconImage.rectTransform.sizeDelta = new Vector2(160, 160);
                break;
            case "Rust_NPC_3":
                iconImage.rectTransform.sizeDelta = new Vector2(151, 169);
                break;
            case "Minimap":
                iconImage.rectTransform.sizeDelta = new Vector2(180, 180);
                break;
            case "NPC_1":
                iconImage.rectTransform.sizeDelta = new Vector2(173, 152);
                break;
            default:
                iconImage.rectTransform.sizeDelta = new Vector2(230, 200);
                break;
        }
    }

    public void ShowCard()
    {
        isSelected = true;

        Tween.LocalRotation(transform, new Vector3(0f, 90f, 0f), 0.1f)
            .OnComplete(() =>
            {
                iconImage.sprite = iconSprite;
                Tween.LocalRotation(transform, new Vector3(0f, 180f, 0f), 0.1f);
            });
    }

    public void HideCard()
    {
        Tween.LocalRotation(transform, new Vector3(0f, 90f, 0f), 0.1f)
            .OnComplete(() =>
            {
                iconImage.sprite = hiddenIconSprite;
                Tween.LocalRotation(transform, Vector3.zero, 0.1f)
                    .OnComplete(() => isSelected = false);
            });
    }
}