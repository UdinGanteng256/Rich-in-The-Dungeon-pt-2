using UnityEngine;

public static class GlobalData
{
    public static ItemInstance[,] savedInventoryGrid;
    public static ItemInstance[] savedHotbarItems = new ItemInstance[4]; 
    public static bool hasDataInitialized = false;
    public static int savedMoney = 0;
    public static float savedStamina = 100f;
    public static void ResetData()
    {
        savedMoney = 0;
        savedStamina = 100f;

        savedInventoryGrid = null; 
        savedHotbarItems = new ItemInstance[4]; 

        hasDataInitialized = false;

        Debug.Log("Global Data Has Reset!");
    }

    public static void EnsureInventoryInitialized(int width, int height)
    {
        if (savedInventoryGrid == null ||
            savedInventoryGrid.GetLength(0) != width ||
            savedInventoryGrid.GetLength(1) != height)
        {
            savedInventoryGrid = new ItemInstance[width, height];
        }
    }
}