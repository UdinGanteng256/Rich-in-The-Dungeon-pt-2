using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "RichInDungeon/Item")]
public class ItemData : ScriptableObject
{
    [Header("Visuals")]
    public string id;
    public Sprite icon;

    [Header("Grid Shape")]
    // Width and Height of the item in grid cells
    public int width = 1;
    public int height = 1;

    // We flatten the 2D shape into a 1D array for easier Inspector editing
    // Example: A 2x2 L-shape would be defined by a bool array.
    // In the Inspector, we can check boxes to define the shape.
    public bool[] shapeLayout; 

    // Helper to get the 2D shape at runtime
    public bool IsOccupied(int x, int y)
    {
        int index = y * width + x;
        if (index >= 0 && index < shapeLayout.Length)
            return shapeLayout[index];
        return false;
    }
}