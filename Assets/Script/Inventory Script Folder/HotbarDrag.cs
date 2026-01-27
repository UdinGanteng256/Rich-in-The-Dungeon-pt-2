using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HotbarDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector3 originalPos;
    private Transform originalParent;

    private ActiveSlot myActiveSlot;
    private InventoryUI inventoryUI;
    private bool isDragging = false;
    
    public bool dropSuccessful = false; 

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        canvas = GetComponentInParent<Canvas>();
        myActiveSlot = GetComponentInParent<ActiveSlot>();
        inventoryUI = FindObjectOfType<InventoryUI>();
    }

    public ActiveSlot GetSourceSlot()
    {
        return myActiveSlot;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (myActiveSlot.GetItem() == null) return;

        dropSuccessful = false;
        isDragging = true;
        originalPos = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        // Bikin agak transparan pas didrag
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        
        // Pindah ke Canvas biar ada di layer paling atas
        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {        
        if (myActiveSlot.GetItem() == null || !isDragging) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        
        // [FIX PENTING] Kembalikan Alpha ke 1 (Normal) APAPUN YANG TERJADI
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // 1. Jika Swap Sukses (diurus oleh ActiveSlot lain)
        if (dropSuccessful)
        {
            // Kembalikan parent ke slot asalnya
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
            
            // Paksa update visual tangan
            UpdatePlayerHand(); 
            return;
        }

        // 2. Logic kembalikan ke Inventory kalau didrop di tanah
        Vector2 mousePos = Mouse.current.position.ReadValue();
        ItemInstance item = myActiveSlot.GetItem();

        if (item != null)
        {
            bool success = inventoryUI.TryAddFromHotbar(item, mousePos);

            if (success)
            {
                Debug.Log("Item dikembalikan ke Inventory.");
                myActiveSlot.ClearSlot();
                UpdatePlayerHand();
            }
            else
            {
                // Kalau gagal masuk inventory (penuh/invalid), balik ke slot hotbar
                transform.SetParent(originalParent);
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }
        else
        {
            // Safety net
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    void UpdatePlayerHand()
    {
        PlayerAction playerAction = FindObjectOfType<PlayerAction>();
        if (playerAction != null) playerAction.ForceUpdateVisuals();
    }
}