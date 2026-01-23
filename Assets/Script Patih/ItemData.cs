using UnityEngine;

// Kategori sesuai requestmu
public enum ItemType
{
    Resource,   // Emas, Batu (Untuk dijual/crafting)
    Tool,       // Pickaxe (Untuk nambang)
    Food        // Makanan (Untuk nambah stamina)
}

[CreateAssetMenu(menuName = "Inventory System/Item Data")]
public class ItemData : ScriptableObject
{
    public string id;
    public string displayName;
    [TextArea(3, 3)] public string description;
    
    [Header("Visual & Size")]
    public Sprite icon;
    public int width = 1;
    public int height = 1;
    
    [Header("Game Data")]
    public ItemType itemType;
    
    // Nilai kegunaan item
    // Jika Tool = Damage ke batu
    // Jika Food = Jumlah Stamina yang diisi
    // Jika Resource = Harga Jual
    public int valueAmount; 

    public bool[,] GetShape()
    {
        return new bool[width, height]; 
    }
    
    public bool IsOccupied(int x, int y)
    {
       return true; 
    }
}