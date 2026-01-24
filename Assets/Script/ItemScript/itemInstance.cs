using UnityEngine;
using System;

[Serializable]
public class ItemInstance
{
    public ItemData itemData;           // Referensi ke ScriptableObject
    public int width;                   // Size unik untuk instance ini
    public int height;
    public int calculatedValue;         // Harga berdasarkan size
    public string instanceId;           // ID unik untuk instance ini
    
    // Constructor dengan size random
    public ItemInstance(ItemData data)
    {
        itemData = data;
        instanceId = Guid.NewGuid().ToString();
        
        // Generate random size berdasarkan item type
        GenerateRandomSize();
        
        // Hitung harga berdasarkan luas
        CalculateValue();
    }
    
    // Constructor dengan size spesifik
    public ItemInstance(ItemData data, int w, int h)
    {
        itemData = data;
        width = w;
        height = h;
        instanceId = Guid.NewGuid().ToString();
        CalculateValue();
    }
    
    void GenerateRandomSize()
    {
        // Size berbeda berdasarkan tipe item
        switch (itemData.itemType)
        {
            case ItemType.Resource:
                int oreSize = UnityEngine.Random.Range(1, 5); 
                width = oreSize;
                height = oreSize;
                break;
                
            case ItemType.Tool:
                width = itemData.width;
                height = itemData.height;
                break;
                
            case ItemType.Food:
                width = UnityEngine.Random.Range(1, 3);
                height = UnityEngine.Random.Range(1, 3);
                break;
        }
    }
    
    void CalculateValue()
    {
        // Harga = base value x luas area
        int area = width * height;
        calculatedValue = itemData.valueAmount * area;
    }
    
    public bool IsOccupied(int x, int y)
    {
        return x < width && y < height;
    }
}