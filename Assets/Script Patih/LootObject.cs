using UnityEngine;
using UnityEngine.InputSystem; // New Input System

public class LootObject : MonoBehaviour
{
    [Header("Isi Loot")]
    public ItemData itemData;

    private InventoryGrid inventoryBackend;
    private InventoryUI inventoryUI;
    private Camera mainCamera;

    private void Start()
    {
        inventoryBackend = FindObjectOfType<InventoryGrid>();
        inventoryUI = FindObjectOfType<InventoryUI>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Deteksi klik kiri mouse (New Input System)
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckClick();
        }
    }

    void CheckClick()
    {
        // Konversi posisi mouse dari layar ke world space
        Vector2 mousePos =
            mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Raycast 2D tepat di posisi mouse
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        // Jika yang terkena raycast adalah loot ini sendiri
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            CollectItem();
        }
    }

    void CollectItem()
    {
        bool berhasilMasuk = inventoryBackend.AutoAddItem(itemData);

        if (berhasilMasuk)
        {
            inventoryUI.RefreshInventoryItems();
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory penuh!");
        }
    }
}
