using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Round attributes")]
    [Min(3)]
    [SerializeField] private int numRounds = 5;
    [SerializeField] private float delayLowerBound = 1f;
    [SerializeField] private float delayUpperBound = 5f;

    [Header("Game elements")]
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject playButton;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private GameObject canvas;

    private double showTime;
    private List<double> reactionTimes = new();
    private GameObject player;
    private GameObject barrier;

    // Called when player enters the collider
    public void StartGame(GameObject playerObj, GameObject barrierObj)
    {
        player = playerObj;
        barrier = barrierObj;

        // Freeze player
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // Show canvas and play button only
        canvas.SetActive(true);
        playButton.SetActive(true);
        scoreText.gameObject.SetActive(false);
        target.SetActive(false);
    }

    // Called when player clicks play button
    public void OnPlayButtonClicked()
    {
        playButton.SetActive(false);
        scoreText.gameObject.SetActive(false);
        reactionTimes.Clear();
        StartCoroutine(PlayRound());
    }

    private IEnumerator PlayRound()
    {
        float delay = Random.Range(delayLowerBound, delayUpperBound);
        yield return new WaitForSecondsRealtime(delay);

        showTime = Time.unscaledTimeAsDouble;

        RectTransform canvasRect = target.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        RectTransform targetRect = target.GetComponent<RectTransform>();

        float halfW = (canvasRect.sizeDelta.x / 2) - 60f; // 60 = padding so it doesn't go off edge
        float halfH = (canvasRect.sizeDelta.y / 2) - 60f;

        targetRect.anchoredPosition = new Vector2(
            Random.Range(-halfW, halfW),
            Random.Range(-halfH, halfH)
        );

        // Make sure anchor is centered
        targetRect.anchorMin = new Vector2(0.5f, 0.5f);
        targetRect.anchorMax = new Vector2(0.5f, 0.5f);
        targetRect.pivot = new Vector2(0.5f, 0.5f);

        target.SetActive(true);
    }
    public void TargetHit()
    {
        reactionTimes.Add(Time.unscaledTimeAsDouble - showTime);
        target.SetActive(false);

        if (reactionTimes.Count < numRounds)
        {
            StartCoroutine(PlayRound());
        }
        else
        {
            double average = (reactionTimes.Sum() - reactionTimes.Min() - reactionTimes.Max()) / (numRounds - 2);
            int milliseconds = (int)(average * 1000);

            if (milliseconds < 500)
                Pass(milliseconds, "Outrageous");
            else if (milliseconds < 800)
                Pass(milliseconds, "Average");
            else
            {
                scoreText.text = $"You can do better! — {milliseconds}ms, Try again!";
                scoreText.gameObject.SetActive(true);
                StartCoroutine(RetryAfterDelay());
            }
        }
    }

    private void Pass(int milliseconds, string scoreType)
    {
        scoreText.text = $"{scoreType} — {milliseconds}ms!";
        scoreText.gameObject.SetActive(true);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (barrier != null)
            barrier.SetActive(false);

        StartCoroutine(HideCanvasAfterDelay());
    }

    private IEnumerator HideCanvasAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2f);
        canvas.SetActive(false);
    }

    private IEnumerator RetryAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2f);
        scoreText.gameObject.SetActive(false);
        reactionTimes.Clear();
        StartCoroutine(PlayRound());
    }
}