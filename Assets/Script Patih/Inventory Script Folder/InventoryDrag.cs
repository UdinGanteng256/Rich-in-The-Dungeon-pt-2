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
    
    private ItemData myItemData;
    private float tileSizeX;
    private float tileSizeY;

    // --- FITUR BARU: Bendera Sukses ---
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

    public ItemData GetItemData()
    {
        return myItemData;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Reset status sukses setiap kali mulai drag
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

        myItemData = inventoryBackend.GetItemAt(gridX, gridY);

        // Hapus sementara dari backend saat diangkat
        if (myItemData != null)
        {
            inventoryBackend.RemoveItem(myItemData, gridX, gridY);
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
        
        // --- CEK APAKAH SUDAH DITERIMA ACTIVE SLOT? ---
        if (dropSuccessful)
        {
            // Kalau sukses diterima Slot Active, HANCURKAN objek drag ini.
            // (Karena visualnya sudah pindah ke icon di Slot Active)
            Destroy(gameObject); 
            return; // Stop di sini, jangan jalankan kode di bawah
        }

        // --- KALO GAK DITERIMA DI MANA-MANA, BARU CEK GRID ---
        transform.SetParent(originalParent);

        int newX = Mathf.RoundToInt(rectTransform.anchoredPosition.x / tileSizeX);
        int newY = Mathf.RoundToInt(-rectTransform.anchoredPosition.y / tileSizeY);

        if (inventoryBackend.CheckAvailableSpace(newX, newY, myItemData))
        {
            inventoryBackend.PlaceItem(myItemData, newX, newY);
        }
        else
        {
            // Balik ke posisi asal
            inventoryBackend.PlaceItem(myItemData, gridX, gridY);
        }

        inventoryUI.RefreshInventoryItems();
    }
}