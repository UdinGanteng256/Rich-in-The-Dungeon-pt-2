using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private InventoryUI inventoryUI;
    private InventoryGrid inventoryBackend;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private RectTransform parentRect; 
    
    public int gridX; 
    public int gridY;
    private Vector2 grabOffset;
    
    private ItemInstance myItemInstance;
    private float tileSizeX;
    private float tileSizeY;

    public bool dropSuccessful = false; 

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        inventoryUI = GetComponentInParent<InventoryUI>();
        if(inventoryUI == null) inventoryUI = FindObjectOfType<InventoryUI>();
        inventoryBackend = inventoryUI.inventoryBackend;
    }

    public void SetGridPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
    }

    public void SetItemInstance(ItemInstance instance)
    {
        myItemInstance = instance;
    }

    public ItemInstance GetItemInstance()
    {
        return myItemInstance;
    }

    // ... (Bagian atas script sama) ...

    public void OnPointerClick(PointerEventData eventData)
    {
        // Cek Klik Kanan
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            MerchantSystem merchant = FindObjectOfType<MerchantSystem>();
            
            // Pastikan Merchant aktif (Bisa mode Jual atau Beli, player bebas jual kapan aja selama di merchant)
            if (merchant != null && merchant.isTradingActive)
            {
                // --- PENGECEKAN PICKAXE / ITEM PENTING ---
                if (myItemInstance.itemData.isSellable == false)
                {
                    Debug.Log("Item ini spesial/penting, tidak bisa dijual!");
                    return; // Batalkan proses jual
                }

                // Kalau lolos, baru jual
                merchant.SellItem(myItemInstance);

                inventoryBackend.RemoveItem(myItemInstance, gridX, gridY);
                Destroy(gameObject);
                inventoryUI.RefreshInventoryItems();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dropSuccessful = false; 

        originalParent = transform.parent;
        parentRect = originalParent.GetComponent<RectTransform>();
        
        GridLayoutGroup gridLayout = inventoryUI.gridContainer.GetComponent<GridLayoutGroup>();
        tileSizeX = gridLayout.cellSize.x + gridLayout.spacing.x;
        tileSizeY = gridLayout.cellSize.y + gridLayout.spacing.y;

        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, eventData.position, null, out localMousePos
        );
        grabOffset = rectTransform.anchoredPosition - localMousePos;

        myItemInstance = inventoryBackend.GetItemAt(gridX, gridY);

        if (myItemInstance != null)
        {
            inventoryBackend.RemoveItem(myItemInstance, gridX, gridY);
        }

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(originalParent.parent); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localMousePos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, null, out localMousePos))
        {
            Vector2 targetPos = localMousePos + grabOffset;
            float snappedX = Mathf.Round(targetPos.x / tileSizeX) * tileSizeX;
            float snappedY = Mathf.Round(targetPos.y / tileSizeY) * tileSizeY;
            rectTransform.anchoredPosition = new Vector2(snappedX, snappedY);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        if (dropSuccessful)
        {
            Destroy(gameObject); 
            return;
        }

        transform.SetParent(originalParent);

        int newX = Mathf.RoundToInt(rectTransform.anchoredPosition.x / tileSizeX);
        int newY = Mathf.RoundToInt(-rectTransform.anchoredPosition.y / tileSizeY);

        if (inventoryBackend.CheckAvailableSpace(newX, newY, myItemInstance))
        {
            inventoryBackend.PlaceItem(myItemInstance, newX, newY);
        }
        else
        {
            inventoryBackend.PlaceItem(myItemInstance, gridX, gridY);
        }

        inventoryUI.RefreshInventoryItems();
    }
}