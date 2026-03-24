using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
  [Header("Round attributes")]
  [Min(3)]
  [SerializeField] private int numRounds = 5;
  [SerializeField] private float delayLowerBound = 1f;
  [SerializeField] private float delayUpperBound = 5f;

  [Header("Game elements")]
  [SerializeField] private GameObject target;
  [SerializeField] private GameObject playButton;
  [SerializeField] private TMPro.TextMeshProUGUI scoreText;


  private double showTime;
  private List<double> reactionTimes = new();

  public void StartGame() {
    // Hide any UI elements we don't want
    playButton.SetActive(false);
    scoreText.gameObject.SetActive(false);

    // Clear any prior game state
    reactionTimes.Clear();

    // Start the coroutine to show the targets.
    StartCoroutine(PlayRound());
  }

  private IEnumerator PlayRound() {
    // Wait for a random duration before showing the target.
    float delay = Random.Range(delayLowerBound, delayUpperBound);
    yield return new WaitForSeconds(delay);

    // Record the time the target was shown.
    // Unscaled so you can't cheat by modifying the timeScale.
    showTime = Time.unscaledTimeAsDouble;

    // Show the target in a random position.
    target.transform.position = Random.insideUnitCircle * 3f;
    target.SetActive(true);
  }

  public void TargetHit() {
    // Records the reaction time
    reactionTimes.Add(Time.unscaledTimeAsDouble - showTime);

    // Hides the target
    target.SetActive(false);

    // If there are still rounds left, starts the next one
    if (reactionTimes.Count < numRounds) {
      StartCoroutine(PlayRound());
    } else {
      // Calculates the reaction time score
      double average = (reactionTimes.Sum() - reactionTimes.Min() - reactionTimes.Max()) / (numRounds - 2);
      int milliseconds = (int)(average * 1000);

      // Shows the UI
      playButton.SetActive(true);

      // Updates the score 
      string scoreType;
      if (milliseconds < 500) {
        scoreType = "Outrageous";
        
      } else if (milliseconds < 700) {
        scoreType = "Average";
        
      } else {
        scoreType = "You can do better!";
      }
      
      scoreText.text = $"{scoreType} score: {milliseconds}ms";
      scoreText.gameObject.SetActive(true);

      // Records the results in milliseconds
      foreach (double reactionTime in reactionTimes) { 
        Debug.Log(reactionTime);
      }
      Debug.Log($"Score: {milliseconds}");
    }
  }
}