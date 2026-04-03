using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PrimeTween;

public class CardController : MonoBehaviour
{
    [SerializeField] private Cards_Script cardPrefab;
    [SerializeField] private Transform gridtransform;
    [SerializeField] private Sprite[] sprites;

    [SerializeField] private TMP_Text movesText;
    [SerializeField] private TMP_Text pairsText;
    [SerializeField] private TMP_Text timerText;

    private List<Sprite> spritePairs;
    private Cards_Script firstSelected;
    private Cards_Script secondSelected;

    private int movesCount;
    private int pairsFound;
    private float timeElapsed;
    private bool inputLocked;

    private void Start()
    {
        PrepareSprites();
        CreateCards();
        UpdateUI();
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        UpdateUI();
    }

    private void PrepareSprites()
    {
        spritePairs = new List<Sprite>();
        foreach (var s in sprites)
        {
            spritePairs.Add(s);
            spritePairs.Add(s);
        }
        Shuffle(spritePairs);
    }

    private void CreateCards()
    {
        foreach (var s in spritePairs)
        {
            var card = Instantiate(cardPrefab, gridtransform);
            card.Controller = this;
            card.SetIconSprite(s);
        }
    }

    public void SetSelected(Cards_Script card)
    {
        if (inputLocked || card.isSelected) return;

        card.ShowCard();

        if (firstSelected == null)
        {
            firstSelected = card;
            return;
        }

        secondSelected = card;
        movesCount++;
        StartCoroutine(CheckMatch(firstSelected, secondSelected));

        firstSelected = null;
        secondSelected = null;
    }

    private IEnumerator CheckMatch(Cards_Script a, Cards_Script b)
    {
        inputLocked = true;
        yield return new WaitForSecondsRealtime(0.35f);

        if (a.iconSprite == b.iconSprite)
        {
            pairsFound++;

            if (pairsFound >= spritePairs.Count / 2)
            {
                Debug.Log("Minigame finished!");

                // Tell the QuestNPC the minigame is done
                // Works across scenes because QuestNPC is in the
                // main scene which stays loaded additively
                QuestNPC questNPC = FindObjectOfType<QuestNPC>();
                if (questNPC != null)
                    questNPC.OnMinigameComplete();
                else
                    Debug.LogError("CardController: Could not find QuestNPC in scene!");
            }
        }
        else
        {
            a.HideCard();
            b.HideCard();
        }

        inputLocked = false;
    }

    private void Shuffle(List<Sprite> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    private void UpdateUI()
    {
        if (movesText != null)
            movesText.text = $"Moves: {movesCount}";
        if (pairsText != null)
            pairsText.text = $"Pairs: {pairsFound}/{spritePairs.Count / 2}";
        if (timerText != null)
        {
            int sec = Mathf.FloorToInt(timeElapsed);
            timerText.text = $"Time: {sec / 60:00}:{sec % 60:00}";
        }
    }
}