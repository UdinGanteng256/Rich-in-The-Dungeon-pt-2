using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActiveSlot : MonoBehaviour, IDropHandler
{
    [Header("Data Item")]
    public ItemData currentItem; 
    public Image iconImage;
    
    [Header("Visual Selection")]
    public Image highlightBorder; // Tarik gambar Border/Frame kuning ke sini

    private InventoryGrid inventoryBackend;

    void Start()
    {
        inventoryBackend = FindObjectOfType<InventoryGrid>();
        UpdateVisual();
        
        // Matikan border saat game mulai (nanti PlayerAction yang nyalain)
        if(highlightBorder != null) highlightBorder.enabled = false;
    }

    // --- LOGIKA TERIMA BARANG (DRAG & DROP) ---
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        InventoryDrag dragScript = droppedObj.GetComponent<InventoryDrag>();

        if (dragScript != null)
        {
            ItemData newItem = dragScript.GetItemData();

            // KITA TERIMA SEMUA JENIS ITEM (Tool, Food, Resource, dll)
            // 1. Simpan Data
            SetItem(newItem);

            // 2. Lapor ke Script Drag: "Sukses masuk! Jangan balik ke grid."
            dragScript.dropSuccessful = true; 
            
            // 3. Refresh Inventory (Biar slot asal jadi kosong)
            FindObjectOfType<InventoryUI>().RefreshInventoryItems();
        }
    }

    // --- FUNGSI BANTUAN ---
    public void SetItem(ItemData item)
    {
        currentItem = item;
        UpdateVisual();
    }

    public void ClearSlot()
    {
        currentItem = null;
        UpdateVisual();
    }

    // Fungsi untuk nyalain/matiin border kuning
    public void SetHighlight(bool isActive)
    {
        if(highlightBorder != null)
            highlightBorder.enabled = isActive;
    }

    void UpdateVisual()
    {
        if (currentItem != null)
        {
            iconImage.sprite = currentItem.icon;
            iconImage.enabled = true;
            iconImage.color = Color.white;
        }
        else
        {
            iconImage.enabled = false;
        }
    }
}