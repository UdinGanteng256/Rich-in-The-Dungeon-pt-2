using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HotbarDrag : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector3 originalPos;
    private Transform originalParent;

    private ActiveSlot myActiveSlot;
    private InventoryUI inventoryUI;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        canvas = GetComponentInParent<Canvas>();
        myActiveSlot = GetComponentInParent<ActiveSlot>();
        inventoryUI = FindObjectOfType<InventoryUI>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (myActiveSlot.currentItem == null) return;

        originalPos = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // Move to root canvas so it renders on top
        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (myActiveSlot.currentItem == null) return;

        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (myActiveSlot.currentItem == null) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        bool success = inventoryUI.TryAddFromHotbar(
            myActiveSlot.currentItem,
            mousePos
        );

        if (success)
        {
            myActiveSlot.ClearSlot();

            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;

            // Force update player hand visual
            PlayerAction playerAction =
                FindObjectOfType<PlayerAction>();

            if (playerAction != null)
                playerAction.SendMessage(
                    "UpdatePlayerHand",
                    SendMessageOptions.DontRequireReceiver
                );
        }
        else
        {
            // Return to original slot
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
