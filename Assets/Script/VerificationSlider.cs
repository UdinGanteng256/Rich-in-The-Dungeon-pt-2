using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class VerificationSlider : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Settings")]
    [SerializeField] private RectTransform trackArea;
    [SerializeField] private float snapDuration = 0.2f;
    [SerializeField, Range(0f, 1f)] private float unlockThreshold = 0.95f;

    [Header("Visual")]
    [SerializeField] private float hoverScale = 1.1f;

    [Header("Transition")]
    [SerializeField] private CanvasGroup sliderCanvasGroup;
    [SerializeField] private CanvasGroup buttonCanvasGroup;

    [Header("Events")]
    public UnityEvent OnUnlock;

    private RectTransform handleRect;
    private Vector2 startPos;
    private float maxTravel;
    private float dragOffsetX;

    private bool isLocked = true;
    private Coroutine activeRoutine;

    private void Awake()
    {
        handleRect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        startPos = handleRect.anchoredPosition;
        maxTravel = -(trackArea.rect.width - handleRect.rect.width);

        SetButtonState(0f, false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isLocked) return;

        StopRoutine();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            trackArea,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 mousePos
        );

        dragOffsetX = mousePos.x - handleRect.anchoredPosition.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isLocked) return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                trackArea,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 mousePos))
            return;

        float x = mousePos.x - dragOffsetX;
        x = Mathf.Clamp(x, startPos.x + maxTravel, startPos.x);

        handleRect.anchoredPosition = new Vector2(x, startPos.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isLocked) return;

        float progress = Mathf.Clamp01(
            (handleRect.anchoredPosition.x - startPos.x) / maxTravel
        );

        if (progress >= unlockThreshold)
            Unlock();
        else
            activeRoutine = StartCoroutine(SnapTo(startPos));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isLocked) return;
        handleRect.localScale = Vector3.one * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isLocked) return;
        handleRect.localScale = Vector3.one;
    }

    private void Unlock()
    {
        isLocked = false;

        handleRect.localScale = Vector3.one;
        handleRect.anchoredPosition =
            new Vector2(startPos.x + maxTravel, startPos.y);

        OnUnlock?.Invoke();

        if (sliderCanvasGroup && buttonCanvasGroup)
            StartCoroutine(CrossFade());
    }

    private IEnumerator SnapTo(Vector2 target)
    {
        Vector2 from = handleRect.anchoredPosition;
        float time = 0f;

        while (time < snapDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, time / snapDuration);

            handleRect.anchoredPosition = Vector2.Lerp(from, target, t);
            yield return null;
        }

        handleRect.anchoredPosition = target;
    }

    private IEnumerator CrossFade()
    {
        float duration = 0.5f;
        float time = 0f;

        sliderCanvasGroup.blocksRaycasts = false;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            sliderCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            buttonCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        sliderCanvasGroup.alpha = 0f;
        buttonCanvasGroup.alpha = 1f;
        buttonCanvasGroup.interactable = true;
        buttonCanvasGroup.blocksRaycasts = true;
    }

    private void StopRoutine()
    {
        if (activeRoutine == null) return;
        StopCoroutine(activeRoutine);
        activeRoutine = null;
    }

    private void SetButtonState(float alpha, bool active)
    {
        if (!buttonCanvasGroup) return;
        buttonCanvasGroup.alpha = alpha;
        buttonCanvasGroup.interactable = active;
        buttonCanvasGroup.blocksRaycasts = active;
    }
}
