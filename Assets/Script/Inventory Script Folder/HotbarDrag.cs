using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HotbarDrag : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector3 originalPos;
    private Transform originalParent;

    private ActiveSlot myActiveSlot;
    private InventoryUI inventoryUI;
    private bool isDragging = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        canvas = GetComponentInParent<Canvas>();
        myActiveSlot = GetComponentInParent<ActiveSlot>();
        inventoryUI = FindObjectOfType<InventoryUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        
        // PAKE GETTER
        ItemInstance item = myActiveSlot.GetItem();
        if (item == null) return;

        InventoryGrid inventoryBackend = inventoryUI.inventoryBackend;
        bool success = inventoryBackend.AutoAddItem(item);

        if (success)
        {
            Debug.Log($"Item {item.itemData.displayName} dikembalikan ke inventory");
            myActiveSlot.ClearSlot();
            inventoryUI.RefreshInventoryItems();

            PlayerAction playerAction = FindObjectOfType<PlayerAction>();
            if (playerAction != null)
                playerAction.SendMessage("UpdatePlayerHand", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.Log("Inventory penuh! Tidak bisa mengembalikan item.");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (myActiveSlot.GetItem() == null) return;

        isDragging = true;
        originalPos = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {        if (myActiveSlot.GetItem() == null || !isDragging) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ItemInstance item = myActiveSlot.GetItem();
        if (item == null) return;

        isDragging = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        
        bool success = inventoryUI.TryAddFromHotbar(item, mousePos);

        if (success)
        {
            Debug.Log($"Item {item.itemData.displayName} dikembalikan ke inventory");
            myActiveSlot.ClearSlot();

            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;

            PlayerAction playerAction = FindObjectOfType<PlayerAction>();
            if (playerAction != null)
                playerAction.SendMessage("UpdatePlayerHand", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
            Debug.Log("Tidak bisa menempatkan item di sana");
        }
    }
}