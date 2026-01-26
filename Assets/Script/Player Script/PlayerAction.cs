using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    [Header("Hotbar")]
    public ActiveSlot[] hotbarSlots;

    private int selectedSlotIndex = -1;

    [Header("Action Settings")]
    public float miningRange = 2.5f;
    public Transform playerBodyLocation;

    [Header("References")]
    public PlayerStats playerStats;
    public PlayerVisual playerVisual;
    private PlayerMovement playerMovement; 

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Auto-assign references
        if (playerVisual == null) playerVisual = FindObjectOfType<PlayerVisual>();
        if (playerStats == null) playerStats = FindObjectOfType<PlayerStats>();
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
        if (playerBodyLocation == null) playerBodyLocation = transform;

        SelectSlot(-1);
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) ToggleSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) ToggleSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) ToggleSlot(2);

        if (Mouse.current.leftButton.wasPressedThisFrame)
            PerformAction();
    }

    public ActiveSlot GetCurrentActiveSlot()
    {
        if (selectedSlotIndex != -1 && selectedSlotIndex < hotbarSlots.Length)
        {
            return hotbarSlots[selectedSlotIndex];
        }
        return null; 
    }

    void ToggleSlot(int index)
    {
        if (index < 0 || index >= hotbarSlots.Length) return;
        SelectSlot(selectedSlotIndex == index ? -1 : index);
    }

    void SelectSlot(int index)
    {
        selectedSlotIndex = index;
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i].SetHighlight(i == selectedSlotIndex);
        }
        
        UpdatePlayerHand();
        UpdateArmedState(); 
    }

    void UpdatePlayerHand()
    {
        if (playerVisual == null) return;

        if (selectedSlotIndex == -1)
        {
            playerVisual.UpdateHandSprite(null);
            return;
        }

        ActiveSlot currentSlot = hotbarSlots[selectedSlotIndex];
        ItemInstance item = currentSlot.GetItem();
        
        playerVisual.UpdateHandSprite(item?.itemData);
    }

    void UpdateArmedState()
    {
        if (playerMovement == null) return;

        if (selectedSlotIndex == -1)
        {
            playerMovement.SetArmedState(0f);
            return;
        }

        ActiveSlot currentSlot = hotbarSlots[selectedSlotIndex];
        ItemInstance item = currentSlot.GetItem();

        if (item == null || item.itemData == null)
        {
            playerMovement.SetArmedState(0f);
            return;
        }

        // Jika ItemType == Tool, set animasi Armed (1f), jika tidak (0f)
        float isArmed = item.itemData.itemType == ItemType.Tool ? 1f : 0f;
        playerMovement.SetArmedState(isArmed);
    }

    void PerformAction()
    {
        if (selectedSlotIndex == -1) return;

        ActiveSlot currentSlot = hotbarSlots[selectedSlotIndex];
        ItemInstance itemInstance = currentSlot.GetItem();

        if (itemInstance == null || itemInstance.itemData == null) return;

        ItemData itemData = itemInstance.itemData;

        switch (itemData.itemType)
        {
            case ItemType.Food:
                Debug.Log($"Makan {itemData.displayName} (Value: {itemInstance.calculatedValue})");
                
                if (playerStats != null) playerStats.EatFood(itemInstance.calculatedValue);
                
                currentSlot.ClearSlot(); 
                UpdatePlayerHand();
                UpdateArmedState(); 
                break;

            case ItemType.Tool:
                if (playerStats == null) return;

                if (!playerStats.HasStamina())
                {
                    Debug.Log("Stamina habis.");
                    return;
                }

                TryMineRock();
                break;

            case ItemType.Resource:
                Debug.Log($"Resource tidak bisa dipakai.");
                break;
        }
    }

    void TryMineRock()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider == null) return;
        MiningNode rock = hit.collider.GetComponent<MiningNode>();
        if (rock == null) return;

        float distance = Vector2.Distance(playerBodyLocation.position, hit.point);
        if (distance > miningRange) return;

        rock.TakeDamage();
        if (playerStats != null) playerStats.ConsumeStaminaForMining();
    }
}