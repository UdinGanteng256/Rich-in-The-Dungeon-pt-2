using UnityEngine;
using System;


[Serializable]
public class ItemInstance
{
    public ItemData itemData;           
    public int width;                   
    public int height;
    public int calculatedValue;         
    public string instanceId;           
    
    // Constructor
    public ItemInstance(ItemData data)
    {
        itemData = data;
        instanceId = Guid.NewGuid().ToString();
        
        GenerateSize();
        CalculateValue();
    }
    
    public ItemInstance(ItemData data, int w, int h)
    {
        itemData = data;
        width = w;
        height = h;
        instanceId = Guid.NewGuid().ToString();
        CalculateValue();
    }
    
    void GenerateSize()
    {
        switch (itemData.itemType)
        {
            case ItemType.Resource:
                int oreSize = UnityEngine.Random.Range(1, 5); 
                width = oreSize;
                height = oreSize;
                break;
                
            case ItemType.Tool:
                // Tool Tetap
                width = itemData.width;
                height = itemData.height;
                break;
                
            case ItemType.Food:
                width = itemData.width;
                height = itemData.height;
                break;
                
            default:
                width = 1;
                height = 1;
                break;
        }
    }
    
    void CalculateValue()
    {
        int area = width * height;
        calculatedValue = itemData.valueAmount * area; // Pastikan valueAmount di ItemData > 0
    }
}