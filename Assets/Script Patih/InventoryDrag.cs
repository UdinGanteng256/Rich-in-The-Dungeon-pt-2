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

    // Posisi item di grid inventory
    public int gridX;
    public int gridY;

    // Offset antara mouse dan posisi item saat pertama di-drag
    private Vector2 grabOffset;

    // Data item yang sedang di-drag
    private ItemData myItemData;

    // Ukuran 1 slot grid (cell + spacing)
    private float tileSizeX;
    private float tileSizeY;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        inventoryUI = GetComponentInParent<InventoryUI>();
        if (inventoryUI == null)
            inventoryUI = FindObjectOfType<InventoryUI>();

        inventoryBackend = inventoryUI.inventoryBackend;
    }

    public void SetGridPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        parentRect = originalParent.GetComponent<RectTransform>();

        // Ambil ukuran slot inventory
        GridLayoutGroup gridLayout = inventoryUI.gridContainer.GetComponent<GridLayoutGroup>();
        tileSizeX = gridLayout.cellSize.x + gridLayout.spacing.x;
        tileSizeY = gridLayout.cellSize.y + gridLayout.spacing.y;

        // Hitung offset agar item tidak lompat saat di-drag
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, eventData.position, null, out localMousePos
        );
        grabOffset = rectTransform.anchoredPosition - localMousePos;

        // Ambil data item lalu hapus sementara dari backend
        myItemData = inventoryBackend.GetItemAt(gridX, gridY);
        if (myItemData != null)
        {
            inventoryBackend.RemoveItem(myItemData, gridX, gridY);
        }

        // Efek visual saat drag
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // Pindahkan ke layer UI paling atas
        transform.SetParent(originalParent.parent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localMousePos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, eventData.position, null, out localMousePos))
        {
            // Posisi target mengikuti mouse + offset
            Vector2 targetPos = localMousePos + grabOffset;

            // Snap ke grid (efek magnet)
            float snappedX = Mathf.Round(targetPos.x / tileSizeX) * tileSizeX;
            float snappedY = Mathf.Round(targetPos.y / tileSizeY) * tileSizeY;

            rectTransform.anchoredPosition = new Vector2(snappedX, snappedY);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(originalParent);

        // Konversi posisi UI ke koordinat grid
        int newX = Mathf.RoundToInt(rectTransform.anchoredPosition.x / tileSizeX);
        int newY = Mathf.RoundToInt(-rectTransform.anchoredPosition.y / tileSizeY);

        // Validasi posisi baru
        if (inventoryBackend.CheckAvailableSpace(newX, newY, myItemData))
        {
            inventoryBackend.PlaceItem(myItemData, newX, newY);
        }
        else
        {
            // Jika gagal, kembalikan ke posisi awal
            inventoryBackend.PlaceItem(myItemData, gridX, gridY);
        }

        inventoryUI.RefreshInventoryItems();
    }
}
