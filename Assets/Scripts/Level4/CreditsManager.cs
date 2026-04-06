using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CreditsManager : MonoBehaviour
{
    [Header("Navigation")]
    public string mainMenuSceneName = "MainMenu";

    [Header("UI References")]
    [Tooltip("A black full-screen Image used for the fade in/out")]
    public Image fadePanel;
    [Tooltip("The RectTransform that contains all credits text and scrolls upward")]
    public RectTransform creditsContainer;

    [Header("Timing")]
    public float fadeInDuration = 2f;
    public float scrollDuration = 18f;
    public float holdAtEndDuration = 3f;
    public float fadeOutDuration = 2f;

    [Header("Game Title")]
    public string gameTitle = "On A Mission";

    [Header("Thank You Message")]
    [TextArea(3, 6)]
    public string thankYouMessage = "Thank you for playing!\n\nDeveloped as a Diplomarbeit project\nat HTL Shkodra, 2025/2026.";

    [Header("Credits")]
    [Tooltip("Add one entry per person. Name and Role are shown separately.")]
    public CreditEntry[] credits = new CreditEntry[]
    {
        new CreditEntry { sectionHeader = "Project Leadership", name = "Henri Mlika", role = "Projektleiter — Programmierung (Gegner, Power-Ups, Inventar, Checkpoints, Level, Musik)" },
        new CreditEntry { name = "Boran Hadri", role = "Stellvertretender Projektleiter — Design (Corporate Design, Charaktere, NPCs, Soundeffekte, Trailer), Programmierung (Minispiele)" },
        new CreditEntry { sectionHeader = "Team Members", name = "Andrea Hila", role = "Story & Programmierung (Geschichte, Spielerbewegungen, UI, NPC-Verhalten, Tutorial & Level-Design)" },
        new CreditEntry { name = "Gloria Lazri", role = "Design & Animation (Umgebungsdesign, Gegnerdesign, UI-Gestaltung, Menüs, Animationen), Programmierung (Quests)" },
        new CreditEntry { sectionHeader = "Supervised by", name = "Bekim Alibali", role = "Projektbetreuer" },
    };

    [Header("Font Settings")]
    public TMP_FontAsset font;
    public float titleFontSize = 52f;
    public float sectionHeaderFontSize = 24f;
    public float nameFontSize = 28f;
    public float roleFontSize = 20f;
    public float thankYouFontSize = 22f;
    public Color titleColor = Color.white;
    public Color sectionHeaderColor = new Color(0.6f, 0.85f, 1f);
    public Color nameColor = Color.white;
    public Color roleColor = new Color(0.8f, 0.8f, 0.8f);
    public Color thankYouColor = Color.white;
    public float spacingBetweenEntries = 40f;

    [Header("Skip")]
    [Tooltip("Player can press this key to skip to the end")]
    public KeyCode skipKey = KeyCode.Escape;

    private bool skipping = false;

    // ─────────────────────────────────────────
    private void Start()
    {
        BuildCreditsUI();
        StartCoroutine(RunCredits());
    }

    private void Update()
    {
        if (Input.GetKeyDown(skipKey) && !skipping)
        {
            skipping = true;
            StopAllCoroutines();
            StartCoroutine(FadeOutAndLoad());
        }
    }

    // ─────────────────────────────────────────
    //  Build credits text
    // ─────────────────────────────────────────
    private void BuildCreditsUI()
    {
        if (creditsContainer == null) return;

        VerticalLayoutGroup vlg = creditsContainer.GetComponent<VerticalLayoutGroup>();
        if (vlg == null) vlg = creditsContainer.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 8f;

        ContentSizeFitter csf = creditsContainer.GetComponent<ContentSizeFitter>();
        if (csf == null) csf = creditsContainer.gameObject.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        AddText(gameTitle, titleFontSize, titleColor, bold: true, spacingAfter: 80f);
        AddSpacer(60f);

        string lastSection = "";

        foreach (var entry in credits)
        {
            if (!string.IsNullOrEmpty(entry.sectionHeader) && entry.sectionHeader != lastSection)
            {
                AddSpacer(spacingBetweenEntries);
                AddText(entry.sectionHeader.ToUpper(), sectionHeaderFontSize, sectionHeaderColor, bold: true, spacingAfter: 10f);
                lastSection = entry.sectionHeader;
            }

            if (!string.IsNullOrEmpty(entry.name))
                AddText(entry.name, nameFontSize, nameColor, bold: true, spacingAfter: 4f);

            if (!string.IsNullOrEmpty(entry.role))
                AddText(entry.role, roleFontSize, roleColor, bold: false, spacingAfter: spacingBetweenEntries * 0.5f);
        }

        AddSpacer(80f);
        AddText(thankYouMessage, thankYouFontSize, thankYouColor, bold: false, spacingAfter: 0f);
        AddSpacer(200f);
    }

    private void AddText(string text, float fontSize, Color color, bool bold, float spacingAfter)
    {
        GameObject go = new GameObject("CreditText");
        go.transform.SetParent(creditsContainer, false);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
        tmp.enableWordWrapping = true;

        if (font != null) tmp.font = font;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(creditsContainer.rect.width > 0 ? creditsContainer.rect.width : 900f, 0);
        tmp.ForceMeshUpdate();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, tmp.preferredHeight + spacingAfter);
    }

    private void AddSpacer(float height)
    {
        GameObject go = new GameObject("Spacer");
        go.transform.SetParent(creditsContainer, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(100f, height);
    }

    // ─────────────────────────────────────────
    //  Credits sequence
    // ─────────────────────────────────────────
    private IEnumerator RunCredits()
    {
        if (creditsContainer != null)
            creditsContainer.gameObject.SetActive(false);

        SetFadeAlpha(1f);

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));

        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(creditsContainer);

        // Get the canvas to calculate proper screen-space position
        Canvas canvas = creditsContainer.GetComponentInParent<Canvas>();
        float canvasScale = canvas != null ? canvas.scaleFactor : 1f;
        float screenHeight = Screen.height / canvasScale;

        Vector3 startPos = creditsContainer.localPosition;
        startPos.y = -(screenHeight * 0.5f) - (creditsContainer.rect.height * 0.5f);
        creditsContainer.localPosition = startPos;

        creditsContainer.gameObject.SetActive(true);

        yield return StartCoroutine(ScrollCredits());

        yield return new WaitForSeconds(holdAtEndDuration);

        yield return StartCoroutine(FadeOutAndLoad());
    }

    private IEnumerator ScrollCredits()
    {
        if (creditsContainer == null) yield break;

        LayoutRebuilder.ForceRebuildLayoutImmediate(creditsContainer);

        Canvas canvas = creditsContainer.GetComponentInParent<Canvas>();
        float canvasScale = canvas != null ? canvas.scaleFactor : 1f;
        float screenHeight = Screen.height / canvasScale;
        float totalHeight = creditsContainer.rect.height;

        float startY = -(screenHeight * 0.5f) - (totalHeight * 0.5f); // fully below screen
        float endY = (screenHeight * 0.5f) + (totalHeight * 0.5f);    // fully above screen

        float elapsed = 0f;
        while (elapsed < scrollDuration && !skipping)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / scrollDuration);
            SetCreditsPosition(Mathf.Lerp(startY, endY, t));
            yield return null;
        }

        SetCreditsPosition(endY);
    }

    private IEnumerator FadeOutAndLoad()
    {
        yield return StartCoroutine(Fade(0f, 1f, fadeOutDuration));
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetFadeAlpha(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        SetFadeAlpha(to);
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = alpha;
            fadePanel.color = c;
        }
    }

    private void SetCreditsPosition(float y)
    {
        if (creditsContainer != null)
        {
            Vector3 pos = creditsContainer.localPosition;
            pos.y = y;
            creditsContainer.localPosition = pos;
        }
    }
}

[System.Serializable]
public class CreditEntry
{
    [Tooltip("Optional section header shown above this entry (e.g. 'Project Leadership')")]
    public string sectionHeader;
    [Tooltip("Person's full name")]
    public string name;
    [TextArea(2, 4)]
    public string role;
}