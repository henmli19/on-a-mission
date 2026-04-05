using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Image fillBar;
    public Image delayedFillBar;
    public TMP_Text bossNameText;

    [Header("Settings")]
    public string bossName = "SENTINEL-X";
    public float delayedDrainSpeed = 0.8f;
    public float flashDuration = 0.12f;

    [Header("Intro Animation")]
    public float slideInDuration = 0.6f;
    public AnimationCurve slideInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // ── Internal ──────────────────────────────
    private float maxHealth;
    private float currentHealth;
    private float delayedFill = 1f;
    private Color originalFillColor;
    private RectTransform rootRect;
    private Vector2 hiddenAnchorPos;
    private Vector2 shownAnchorPos;
    private Coroutine flashCoroutine;
    private bool initialized = false;

    // ─────────────────────────────────────────
    private void Awake()
    {
        rootRect = GetComponent<RectTransform>();
        shownAnchorPos  = rootRect.anchoredPosition;
        hiddenAnchorPos = shownAnchorPos + new Vector2(0, 150f);

        if (bossNameText != null)
            bossNameText.text = bossName;

        if (fillBar != null)
            originalFillColor = fillBar.color;
    }

    // ─────────────────────────────────────────
    public void SetMaxHealth(float max)
    {
        maxHealth     = max;
        currentHealth = max;
        delayedFill   = 1f;
        initialized   = true;

        if (fillBar        != null) fillBar.fillAmount        = 1f;
        if (delayedFillBar != null) delayedFillBar.fillAmount = 1f;

        if (bossNameText != null)
            bossNameText.text = bossName;

        StartCoroutine(SlideIn());
    }

    // ─────────────────────────────────────────
    public void SetHealth(float health)
    {
        // Don't do anything if the bar is already hidden
        if (!gameObject.activeInHierarchy) return;

        currentHealth = Mathf.Clamp(health, 0, maxHealth);

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(HitFlash());
    }

    // ─────────────────────────────────────────
    public void HideBar()
    {
        StartCoroutine(SlideOut());
    }

    // ─────────────────────────────────────────
    private void Update()
    {
        if (!initialized) return;

        float targetFill = Mathf.Clamp01(currentHealth / maxHealth);

        // Main bar always snaps to current health immediately
        if (fillBar != null)
            fillBar.fillAmount = targetFill;

        // Delayed bar slowly catches up to the main bar
        if (delayedFillBar != null && delayedFill > targetFill)
        {
            delayedFill = Mathf.MoveTowards(
                delayedFill, targetFill, delayedDrainSpeed * Time.deltaTime
            );
            delayedFillBar.fillAmount = delayedFill;
        }
    }

    // ─────────────────────────────────────────
    private IEnumerator SlideIn()
    {
        rootRect.anchoredPosition = hiddenAnchorPos;
        float elapsed = 0f;

        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;
            float t = slideInCurve.Evaluate(Mathf.Clamp01(elapsed / slideInDuration));
            rootRect.anchoredPosition = Vector2.Lerp(hiddenAnchorPos, shownAnchorPos, t);
            yield return null;
        }

        rootRect.anchoredPosition = shownAnchorPos;
    }

    private IEnumerator SlideOut()
    {
        float elapsed  = 0f;
        Vector2 startPos = rootRect.anchoredPosition;

        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideInDuration;
            rootRect.anchoredPosition = Vector2.Lerp(startPos, hiddenAnchorPos, t);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private IEnumerator HitFlash()
    {
        if (fillBar == null) yield break;
        fillBar.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        fillBar.color = originalFillColor;
    }
}