using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class SliderScript2 : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Settings")]
    [SerializeField] private RectTransform trackArea;
    [SerializeField] private float snapDuration = 0.2f;
    [SerializeField] private float unlockThreshold = 0.95f;

    [Header("Target Button")]
    [SerializeField] private CanvasGroup buttonCanvasGroup;

    [Header("Events")]
    public UnityEvent OnUnlock;
    public UnityEvent OnLock;

    private RectTransform handleRect;

    private Vector2 startPos;
    private Vector2 endPos;
    private float maxTravel;

    private float dragOffsetX;
    private Coroutine snapRoutine;

    private void Awake()
    {
        handleRect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        startPos = handleRect.anchoredPosition;

        float trackWidth = trackArea.rect.width;
        float handleWidth = handleRect.rect.width;

        maxTravel = -(trackWidth - handleWidth);
        endPos = new Vector2(startPos.x + maxTravel, startPos.y);

        SetButtonState(0f, false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        StopSnap();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            trackArea,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMouse
        );

        dragOffsetX = localMouse.x - handleRect.anchoredPosition.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                trackArea,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 mousePos))
            return;

        float x = mousePos.x - dragOffsetX;
        x = Mathf.Clamp(x, endPos.x, startPos.x);

        handleRect.anchoredPosition = new Vector2(x, startPos.y);

        float progress = Mathf.Clamp01((x - startPos.x) / maxTravel);
        SetButtonAlpha(progress);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float progress = Mathf.Clamp01(
            (handleRect.anchoredPosition.x - startPos.x) / maxTravel
        );

        if (progress >= unlockThreshold)
        {
            snapRoutine = StartCoroutine(SnapAndFade(endPos, 1f));
            SetButtonInteractable(true);
            OnUnlock?.Invoke();
        }
        else
        {
            snapRoutine = StartCoroutine(SnapAndFade(startPos, 0f));
            SetButtonInteractable(false);
            OnLock?.Invoke();
        }
    }

    private IEnumerator SnapAndFade(Vector2 targetPos, float targetAlpha)
    {
        Vector2 fromPos = handleRect.anchoredPosition;
        float fromAlpha = buttonCanvasGroup ? buttonCanvasGroup.alpha : 0f;
        float time = 0f;

        while (time < snapDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, time / snapDuration);

            handleRect.anchoredPosition = Vector2.Lerp(fromPos, targetPos, t);
            SetButtonAlpha(Mathf.Lerp(fromAlpha, targetAlpha, t));

            yield return null;
        }

        handleRect.anchoredPosition = targetPos;
        SetButtonAlpha(targetAlpha);
    }

    private void StopSnap()
    {
        if (snapRoutine == null) return;
        StopCoroutine(snapRoutine);
        snapRoutine = null;
    }

    private void SetButtonAlpha(float alpha)
    {
        if (!buttonCanvasGroup) return;
        buttonCanvasGroup.alpha = alpha;
    }

    private void SetButtonInteractable(bool state)
    {
        if (!buttonCanvasGroup) return;
        buttonCanvasGroup.interactable = state;
        buttonCanvasGroup.blocksRaycasts = state;
    }

    private void SetButtonState(float alpha, bool interactable)
    {
        if (!buttonCanvasGroup) return;
        buttonCanvasGroup.alpha = alpha;
        buttonCanvasGroup.interactable = interactable;
        buttonCanvasGroup.blocksRaycasts = interactable;
    }
}
