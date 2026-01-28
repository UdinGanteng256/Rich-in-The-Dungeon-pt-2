using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class LootObject : MonoBehaviour
{
    [Header("Isi Loot")]
    public ItemData itemData;
    
    [Header("Random Size")]
    public bool useRandomSize = true; 
    public int fixedWidth = 1; 
    public int fixedHeight = 1;

    [Header("Visual Info")]
    public TextMeshPro infoText; 
    public SpriteRenderer spriteRenderer;

    private InventoryGrid inventoryBackend;
    private InventoryUI inventoryUI;
    private Camera mainCamera;
    
    private ItemInstance generatedInstance;

    private void Start()
    {
        inventoryBackend = FindObjectOfType<InventoryGrid>();
        inventoryUI = FindObjectOfType<InventoryUI>();
        mainCamera = Camera.main;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Generate instance saat spawn
        GenerateLootInstance();
        UpdateVisualInfo();
    }

    void GenerateLootInstance()
    {
        if (useRandomSize)
        {
            generatedInstance = new ItemInstance(itemData);
        }
        else
        {
            generatedInstance = new ItemInstance(itemData, fixedWidth, fixedHeight);
        }

        Debug.Log($"Loot spawned: {itemData.displayName} [{generatedInstance.width}x{generatedInstance.height}] = ${generatedInstance.calculatedValue}");
    }

    void UpdateVisualInfo()
    {
        if (spriteRenderer != null && itemData.icon != null)
        {
            spriteRenderer.sprite = itemData.icon;
            
            float scaleX = generatedInstance.width * 0.5f;
            float scaleY = generatedInstance.height * 0.5f;
            transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }

        if (infoText != null)
        {
            infoText.text = $"{generatedInstance.width}x{generatedInstance.height}\n${generatedInstance.calculatedValue}";
            infoText.color = Color.yellow;
            infoText.fontSize = 3;
        }
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckClick();
        }
    }

    void CheckClick()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            CollectItem();
        }
    }

    void CollectItem()
    {
        bool berhasilMasuk = inventoryBackend.AutoAddItem(generatedInstance);

        if (berhasilMasuk)
        {
            Debug.Log($"Collected: {itemData.displayName} [{generatedInstance.width}x{generatedInstance.height}] worth ${generatedInstance.calculatedValue}");
            inventoryUI.RefreshInventoryItems();
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory penuh! Tidak bisa mengambil item.");
        }
    }
}