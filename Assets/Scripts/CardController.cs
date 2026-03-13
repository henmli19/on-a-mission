using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using TMPro;
using PrimeTween;

public class CardController : MonoBehaviour {
    
    [Header("Card Setup")]
    [SerializeField] private Cards_Script cardPrefab;     // Prefab for a single card
    [SerializeField] private Transform gridtransform;     // Parent transform for the grid layout
    [SerializeField] private Sprite[] sprites;            // Unique sprites (each will be duplicated to form pairs)

    [Header("UI (TextMeshPro)")]
    [SerializeField] private TMP_Text movesText;          // Displays number of moves
    [SerializeField] private TMP_Text pairsText;          // Displays found pairs
    [SerializeField] private TMP_Text timerText;          // Displays elapsed time
    
    private List<Sprite> spritePairs;                     // List containing all sprites as pairs
    private Cards_Script firstSelected;                   // First selected card in a turn
    private Cards_Script secondSelected;                  // Second selected card in a turn

    private int movesCount;                               // Number of completed moves (2 cards = 1 move)
    private int pairsFound;                               // Number of successfully matched pairs
    private float timeElapsed;                            // Elapsed game time
    private bool gameWon;                                 // Flag to stop input and timer when finished
    private bool inputLocked;                             // Prevents interaction during matching

    private void Start() {
        // Initialize game state
        movesCount = 0;
        pairsFound = 0;
        timeElapsed = 0f;
        gameWon = false;
        inputLocked = false;
        
        PrepareSprites(); // Prepare paired sprite list
        CreateCards(); // Instantiate cards in grid
        UpdateUI(); // Update UI at start

    }

    private void Update() {
        // Stop timer if game is finished
        if (gameWon) return;

        // Increase time every frame
        timeElapsed += Time.deltaTime;
        UpdateUI();
    }

    private void PrepareSprites() {
        // Create a new list where every sprite appears twice
        spritePairs = new List<Sprite>();

        for (int i = 0; i < sprites.Length; i++) {
            spritePairs.Add(sprites[i]);
            spritePairs.Add(sprites[i]);
        }
        
        ShufflePairs(spritePairs); // Shuffle sprites for random distribution
    }

    private void CreateCards() {
        // Instantiate cards and assign sprite + controller reference
        for (int i = 0; i < spritePairs.Count; i++) {
            Cards_Script card = Instantiate(cardPrefab, gridtransform);
            card.Controller = this;                      
            card.SetIconSprite(spritePairs[i]);          
        }
    }

    public void SetSelected(Cards_Script card) {

        if (gameWon || inputLocked) return;     
        if (card.isSelected) return;            
        if (firstSelected == card) return;      

        // Reveal selected card
        card.ShowCard();

        if (firstSelected == null) {
            firstSelected = card;
            return;
        }

        if (secondSelected == null) {
            secondSelected = card;
            
            movesCount++; // One move consists of flipping two cards
            UpdateUI();

            // Start match-check coroutine
            StartCoroutine(CheckMatching(firstSelected, secondSelected));

            firstSelected = null;
            secondSelected = null;
        }
    }

    private IEnumerator CheckMatching(Cards_Script a, Cards_Script b) {

        inputLocked = true;

        // Short delay so player can see both cards
        yield return new WaitForSeconds(0.35f);

        // Compare sprite references
        if (a.iconSprite == b.iconSprite) {
            pairsFound++;
            UpdateUI();

            // Win condition: all pairs found
            if (pairsFound >= spritePairs.Count / 2) {
                gameWon = true;

                // Visual feedback animation
                Sequence.Create()
                    .Chain(Tween.Scale(gridtransform, Vector3.one * 1.05f, 0.2f, ease: Ease.OutBack))
                    .Chain(Tween.Scale(gridtransform, Vector3.one, 0.12f));
            }
        } 
        else {
            // If no match, flip cards back
            a.HideCard();
            b.HideCard();
        }

        inputLocked = false;
    }

    private void ShufflePairs(List<Sprite> list) {
        // Fisher-Yates shuffle algorithm
        for (int i = list.Count - 1; i > 0; i--) {
            int r = Random.Range(0, i + 1);
            Sprite tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    private void UpdateUI() {

        if (movesText != null) 
            movesText.text = $"Moves: {movesCount}";

        if (pairsText != null) 
            pairsText.text = $"Pairs found: {pairsFound}/{spritePairs.Count / 2}";

        if (timerText != null) {
            int seconds = Mathf.FloorToInt(timeElapsed);
            int mins = seconds / 60;
            int secs = seconds % 60;
            timerText.text = $"Time: {mins:00}:{secs:00}";
        }
    }
}
