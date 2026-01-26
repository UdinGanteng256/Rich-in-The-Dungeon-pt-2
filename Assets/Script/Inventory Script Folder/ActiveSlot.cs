using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ActiveSlot : MonoBehaviour, IDropHandler
{
    [Header("Data Item (Internal)")]
    [SerializeField] private ItemInstance currentItem; 
    
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI itemInfoText; 
    
    [Header("Visual Selection")]
    public Image highlightBorder;

    private InventoryGrid inventoryBackend;

    void Start()
    {
        inventoryBackend = FindObjectOfType<InventoryGrid>();
        
        // Pastikan visual update, walau kosong
        UpdateVisual();
        
        if(highlightBorder != null) highlightBorder.enabled = false;
    }

     public ItemInstance GetItem()
    {
        if (currentItem != null && currentItem.itemData == null)
        {
            return null;
        }
        return currentItem;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        InventoryDrag dragScript = droppedObj.GetComponent<InventoryDrag>();

        if (dragScript != null)
        {
            ItemInstance newItem = dragScript.GetItemInstance();
            
            SetItem(newItem);
            
            dragScript.dropSuccessful = true; 
            FindObjectOfType<InventoryUI>().RefreshInventoryItems();
        }
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
        if(highlightBorder != null)
            highlightBorder.enabled = isActive;
    }

    void UpdateVisual()
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
                itemInfoText.text = $"${currentItem.calculatedValue}";
                itemInfoText.enabled = true;
            }
        }
        else
        {
            if (iconImage != null)
            {
                iconImage.enabled = false;
                iconImage.sprite = null; // Hapus referensi gambar lama
            }
                
            if (itemInfoText != null)
                itemInfoText.enabled = false;
        }
    }
}