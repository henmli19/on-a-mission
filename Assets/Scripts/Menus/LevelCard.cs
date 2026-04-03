using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(EventTrigger))]
public class LevelCard : MonoBehaviour
{
    public string sceneName;
    public Image levelImage;
    public RawImage videoOverlay;
    public VideoClip hoverClip;

    private VideoPlayer videoPlayer;
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Coroutine videoTimer;
    private RenderTexture renderTexture;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.clip = hoverClip;

        Rect r = rectTransform.rect;
        renderTexture = new RenderTexture(Mathf.Max(1, (int)Mathf.Abs(r.width)), Mathf.Max(1, (int)Mathf.Abs(r.height)), 0);
        videoPlayer.targetTexture = renderTexture;
        videoOverlay.texture = renderTexture;
        videoOverlay.gameObject.SetActive(false);

        EventTrigger trigger = GetComponent<EventTrigger>();
        AddEntry(trigger, EventTriggerType.PointerEnter, OnHoverEnter);
        AddEntry(trigger, EventTriggerType.PointerExit, OnHoverExit);
        AddEntry(trigger, EventTriggerType.PointerClick, OnClick);
    }

    private void OnHoverEnter(BaseEventData data)
    {
        rectTransform.localScale = originalScale * 1.2f;
        videoPlayer.time = 0;
        videoPlayer.Play();
        videoOverlay.gameObject.SetActive(true);
        levelImage.gameObject.SetActive(false);

        if (videoTimer != null) StopCoroutine(videoTimer);
        videoTimer = StartCoroutine(StopAfterDuration());
    }

    private void OnHoverExit(BaseEventData data)
    {
        rectTransform.localScale = originalScale;
        if (videoTimer != null) StopCoroutine(videoTimer);
        videoPlayer.Stop();
        ShowImage();
    }

    private void OnClick(BaseEventData data)
    {
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator StopAfterDuration()
    {
        yield return new WaitForSeconds(5f);
        videoPlayer.Stop();
        ShowImage();
    }

    private void ShowImage()
    {
        videoOverlay.gameObject.SetActive(false);
        levelImage.gameObject.SetActive(true);
    }

    private void AddEntry(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    private void OnDestroy()
    {
        renderTexture.Release();
        Destroy(renderTexture);
    }
}