using UnityEngine;

public static class GlobalData
{
    // Inventory Tas
    public static ItemInstance[,] savedInventoryGrid;
    public static bool hasDataInitialized = false;

    // Player Stats
    public static int savedMoney = 0;
    public static float savedStamina = 100f;
    public static ItemInstance[] savedHotbarItems = new ItemInstance[4]; 

    // Safety Init
    public static void EnsureInventoryInitialized(int width, int height)
    {
        if (savedInventoryGrid == null ||
            savedInventoryGrid.GetLength(0) != width ||
            savedInventoryGrid.GetLength(1) != height)
        {
            savedInventoryGrid = new ItemInstance[width, height];
        }
    }

    public static void ResetData()
    {
        savedInventoryGrid = null;
        savedHotbarItems = new ItemInstance[4]; 
        savedMoney = 0;
        savedStamina = 100f;
        hasDataInitialized = false;
    }
}