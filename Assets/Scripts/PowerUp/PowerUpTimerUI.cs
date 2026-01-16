using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpTimerUI : MonoBehaviour
{
    [Header("Timer Settings")]
    public float powerUpDuration = 5f;
    public Image[] bars;

    private float segmentDuration;

    void Awake()
    {
        segmentDuration = powerUpDuration / bars.Length;
    }

    public void StartTimer()
    {
        StopAllCoroutines();
        ResetBars();
        StartCoroutine(TimerRoutine());
    }

    IEnumerator TimerRoutine()
    {
        for (int i = bars.Length - 1; i >= 0; i--)
        {
            yield return new WaitForSeconds(segmentDuration);
            bars[i].enabled = false; // or fade out
        }

        PowerUpEnded();
    }

    void ResetBars()
    {
        foreach (var bar in bars)
            bar.enabled = true;
    }

    void PowerUpEnded()
    {
        Debug.Log($"{gameObject.name} Power-Up ended!");
        gameObject.SetActive(false); // optional
    }
}