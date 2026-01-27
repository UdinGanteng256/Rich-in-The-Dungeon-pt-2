using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ActiveSlot : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    [Header("Data Item (Internal)")]
    [SerializeField] private ItemInstance currentItem; 
    public int slotIndex; 
    
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI itemInfoText; 
    
    [Header("Visual Selection")]
    public Image highlightBorder;

    private InventoryGrid inventoryBackend;

    void Start()
    {
        inventoryBackend = FindObjectOfType<InventoryGrid>();
        UpdateVisual();
        if(highlightBorder != null) highlightBorder.enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            PlayerAction playerAction = FindObjectOfType<PlayerAction>();
            if (playerAction != null)
            {
                playerAction.OnHotbarSlotClicked(slotIndex);
            }
        }
    }

    public ItemInstance GetItem()
    {
        // Safety: Jangan return item yang datanya rusak
        if (currentItem != null && currentItem.itemData == null) return null;
        return currentItem;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj == null) return;

        // KASUS 1: Barang dari Inventory -> Hotbar
        InventoryDrag inventoryDrag = droppedObj.GetComponent<InventoryDrag>();
        if (inventoryDrag != null)
        {
            HandleInventoryDrop(inventoryDrag);
            return;
        }

        // KASUS 2: Barang dari Hotbar -> Hotbar (SWAP)
        HotbarDrag hotbarDrag = droppedObj.GetComponent<HotbarDrag>();
        if (hotbarDrag != null)
        {
            HandleHotbarSwap(hotbarDrag);
            return;
        }
    }

    // --- [LOGIC SWAP YANG DIPERBAIKI] ---
    void HandleHotbarSwap(HotbarDrag sourceDragScript)
    {
        ActiveSlot sourceSlot = sourceDragScript.GetSourceSlot();
        
        // 1. Cek Validasi: Jangan swap sama diri sendiri
        if (sourceSlot == this) return;

        Debug.Log($"Melakukan Swap: Slot {sourceSlot.slotIndex} <-> Slot {this.slotIndex}");

        // 2. Simpan data di variabel sementara (Biar aman)
        ItemInstance itemAsal = sourceSlot.GetItem();  // Item yang didrag (misal: Food)
        ItemInstance itemTujuan = this.GetItem();      // Item yang ditimpa (misal: Resource)

        // 3. Lakukan Tukar Posisi
        // Slot Tujuan (Ini) -> Diisi Item Asal
        this.SetItem(itemAsal);

        // Slot Asal -> Diisi Item Tujuan (Resource tadi)
        sourceSlot.SetItem(itemTujuan);

        // 4. Update Visual Slot Asal SECARA PAKSA (Penting!)
        // Kadang slot asal visualnya 'tertinggal' atau alpha-nya error
        sourceSlot.UpdateVisual();

        // 5. Bilang ke script drag kalau sukses (biar alpha balik normal)
        sourceDragScript.dropSuccessful = true;

        // 6. Update tangan player
        PlayerAction pa = FindObjectOfType<PlayerAction>();
        if (pa != null) pa.ForceUpdateVisuals();
    }

    void HandleInventoryDrop(InventoryDrag dragScript)
    {
        ItemInstance newItem = dragScript.GetItemInstance();
        
        // Logic replace: Kembalikan item lama ke inventory
        if (currentItem != null && currentItem.itemData != null)
        {
            if (inventoryBackend == null) inventoryBackend = FindObjectOfType<InventoryGrid>();

            if (inventoryBackend != null)
            {
                bool addedBack = inventoryBackend.AutoAddItem(currentItem);
                if (!addedBack)
                {
                    Debug.Log("Inventory Penuh! Gagal replace item.");
                    return; 
                }
            }
        }
        
        SetItem(newItem);
        dragScript.dropSuccessful = true; 
        
        InventoryUI ui = FindObjectOfType<InventoryUI>();
        if (ui != null) ui.RefreshInventoryItems();
        
        PlayerAction pa = FindObjectOfType<PlayerAction>();
        if (pa != null) pa.ForceUpdateVisuals();
    }

    public void SetItem(ItemInstance item)
    {
        currentItem = item;
        UpdateVisual();
    }

    public void ClearSlot()
    {
        currentItem = null;
        UpdateVisual();
    }

    public void SetHighlight(bool isActive)
    {
        if(highlightBorder != null) highlightBorder.enabled = isActive;
    }

    // Ubah jadi Public biar bisa dipanggil dari luar saat swap
    public void UpdateVisual()
    {
        if (currentItem != null && currentItem.itemData != null)
        {
            if (iconImage != null)
            {
                iconImage.sprite = currentItem.itemData.icon;
                iconImage.enabled = true;
                iconImage.color = Color.white; // Pastikan warnanya normal (tidak transparan)
            }

            if (itemInfoText != null)
            {
                // Tampilkan harga/info jika bukan Tool
                itemInfoText.text = currentItem.itemData.itemType == ItemType.Tool ? "" : $"${currentItem.calculatedValue}";
                itemInfoText.enabled = true;
            }
        }
        else
        {
            // Reset Visual kalau kosong
            if (iconImage != null)
            {
                iconImage.enabled = false;
                iconImage.sprite = null; 
            }
                
            if (itemInfoText != null)
                itemInfoText.enabled = false;
        }
    }
}