using UnityEngine;

public enum ItemType
{
    Resource,   // Item crafting / jual
    Tool,       // Alat (mining, dll)
    Food        // Konsumsi (stamina, dll)
}

[CreateAssetMenu(menuName = "Inventory System/Item Data")]
public class ItemData : ScriptableObject
{
    public string id;
    public string displayName;

    [TextArea(3, 3)]
    public string description;

    [Header("Visual")]
    public Sprite icon;
    public int width = 1;
    public int height = 1;

    [Header("Gameplay")]
    public ItemType itemType;

    // Nilai berdasarkan tipe item
    public int valueAmount;

    private bool[,] cachedShape;

    public bool[,] GetShape()
    {
        if (cachedShape == null)
            cachedShape = new bool[width, height];

        return cachedShape;
    }


    public bool IsOccupied(int x, int y)
    {
        return true;
    }
}
