using UnityEngine;
using System.Collections.Generic;

public class InventoryGrid : MonoBehaviour
{
    // --- SETTINGS ---
    [Header("Settings")]
    public int gridSizeWidth = 8;
    public int gridSizeHeight = 4;

    [Header("Starter Pack")]
    public ItemData startingItem;
    //Data base Lokal  pake global data
    private ItemInstance[,] inventoryGrid;

    private void Awake()
    {
        inventoryGrid = new ItemInstance[gridSizeWidth, gridSizeHeight];
    }

    private void Start()
    {
        if (GlobalData.hasDataInitialized)
        {
            inventoryGrid = GlobalData.savedInventoryGrid;
        }
        else
        {
            if (startingItem != null)
            {
                ItemInstance newItem = new ItemInstance(startingItem);
                AutoAddItem(newItem);
            }
            
            GlobalData.hasDataInitialized = true;
            SaveToGlobal();
        }

        InventoryUI ui = FindObjectOfType<InventoryUI>();
        if (ui != null)
        {
            ui.inventoryBackend = this; 
            ui.RefreshInventoryItems();
        }
    }

    // --- FUNGSI SAVE ---
    public void SaveToGlobal()
    {
        GlobalData.savedInventoryGrid = this.inventoryGrid;
    }

    public ItemInstance GetItemAt(int x, int y)
    {
        if (x >= 0 && x < gridSizeWidth && y >= 0 && y < gridSizeHeight)
            return inventoryGrid[x, y];
        return null;
    }

    public void PlaceItem(ItemInstance item, int posX, int posY)
    {
        for (int x = 0; x < item.width; x++)
        {
            for (int y = 0; y < item.height; y++)
            {
                if (posX + x < gridSizeWidth && posY + y < gridSizeHeight)
                    inventoryGrid[posX + x, posY + y] = item;
            }
        }
        SaveToGlobal(); // Auto Save
    }

    public void RemoveItem(ItemInstance item, int oldX, int oldY)
    {
        if (item == null) return;

        for (int x = 0; x < item.width; x++)
        {
            for (int y = 0; y < item.height; y++)
            {
                if (oldX + x < gridSizeWidth && oldY + y < gridSizeHeight)
                {
                    if (inventoryGrid[oldX + x, oldY + y] == item)
                        inventoryGrid[oldX + x, oldY + y] = null;
                }
            }
        }
        SaveToGlobal(); // Auto Save
    }

    public bool CheckAvailableSpace(int posX, int posY, ItemInstance item)
    {
        if (posX < 0 || posY < 0) return false;
        if (posX + item.width > gridSizeWidth) return false;
        if (posY + item.height > gridSizeHeight) return false;

        for (int x = 0; x < item.width; x++)
        {
            for (int y = 0; y < item.height; y++)
            {
                if (inventoryGrid[posX + x, posY + y] != null) return false;
            }
        }
        return true;
    }

    public bool AutoAddItem(ItemInstance item)
    {
        if (item == null || item.itemData == null) return false;

        for (int y = 0; y < gridSizeHeight; y++)
        {
            for (int x = 0; x < gridSizeWidth; x++)
            {
                if (CheckAvailableSpace(x, y, item))
                {
                    PlaceItem(item, x, y);
                    return true;
                }
            }
        }
        return false;
    }

    public bool AutoAddItem(ItemData itemData)
    {
        return AutoAddItem(new ItemInstance(itemData));
    }

    public List<ItemInstance> GetAllItems()
    {
        List<ItemInstance> items = new List<ItemInstance>();
        HashSet<ItemInstance> processed = new HashSet<ItemInstance>();

        for (int x = 0; x < gridSizeWidth; x++)
        {
            for (int y = 0; y < gridSizeHeight; y++)
            {
                ItemInstance item = inventoryGrid[x, y];
                if (item != null && !processed.Contains(item))
                {
                    items.Add(item);
                    processed.Add(item);
                }
            }
        }
        return items;
    }
}