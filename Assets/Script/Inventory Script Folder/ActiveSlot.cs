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

    public void SetItemSilent(ItemInstance item)
    {
        currentItem = item;
        UpdateVisual();
    }

    public void SetItem(ItemInstance item)
    {
        currentItem = item;
        UpdateVisual();
        
        // Lapor ke HotbarManager buat save
        HotbarManager manager = FindObjectOfType<HotbarManager>();
        if (manager != null) manager.SaveHotbar();
    }

    public void ClearSlot()
    {
        currentItem = null;
        UpdateVisual();

        // Lapor ke HotbarManager buat save
        HotbarManager manager = FindObjectOfType<HotbarManager>();
        if (manager != null) manager.SaveHotbar();
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
        if (currentItem != null && currentItem.itemData == null) return null;
        return currentItem;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj == null) return;

        InventoryDrag inventoryDrag = droppedObj.GetComponent<InventoryDrag>();
        if (inventoryDrag != null)
        {
            HandleInventoryDrop(inventoryDrag);
            return;
        }

        HotbarDrag hotbarDrag = droppedObj.GetComponent<HotbarDrag>();
        if (hotbarDrag != null)
        {
            HandleHotbarSwap(hotbarDrag);
            return;
        }
    }

    void HandleHotbarSwap(HotbarDrag sourceDragScript)
    {
        ActiveSlot sourceSlot = sourceDragScript.GetSourceSlot();
        if (sourceSlot == this) return;

        ItemInstance itemAsal = sourceSlot.GetItem();  
        ItemInstance itemTujuan = this.GetItem();      

        this.SetItem(itemAsal); // Ini trigger save
        sourceSlot.SetItem(itemTujuan); // Ini trigger save

        sourceSlot.UpdateVisual();
        sourceDragScript.dropSuccessful = true;

        PlayerAction pa = FindObjectOfType<PlayerAction>();
        if (pa != null) pa.ForceUpdateVisuals();
    }

    void HandleInventoryDrop(InventoryDrag dragScript)
    {
        ItemInstance newItem = dragScript.GetItemInstance();
        
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
        
        SetItem(newItem); // Ini trigger save
        dragScript.dropSuccessful = true; 
        
        InventoryUI ui = FindObjectOfType<InventoryUI>();
        if (ui != null) ui.RefreshInventoryItems();
        
        PlayerAction pa = FindObjectOfType<PlayerAction>();
        if (pa != null) pa.ForceUpdateVisuals();
    }

    public void SetHighlight(bool isActive)
    {
        if(highlightBorder != null) highlightBorder.enabled = isActive;
    }

    public void UpdateVisual()
    {
        if (currentItem != null && currentItem.itemData != null)
        {
            if (iconImage != null)
            {
                iconImage.sprite = currentItem.itemData.icon;
                iconImage.enabled = true;
                iconImage.color = Color.white;
            }

            if (itemInfoText != null)
            {
                itemInfoText.text = currentItem.itemData.itemType == ItemType.Tool ? "" : $"${currentItem.calculatedValue}";
                itemInfoText.enabled = true;
            }
        }
        else
        {
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