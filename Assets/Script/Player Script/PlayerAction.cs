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

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (playerVisual == null) playerVisual = FindObjectOfType<PlayerVisual>();
        if (playerStats == null) playerStats = FindObjectOfType<PlayerStats>();
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
        
        // Safety check null
        playerVisual.UpdateHandSprite(item?.itemData);
    }

    void PerformAction()
    {
        if (selectedSlotIndex == -1) return;

        ActiveSlot currentSlot = hotbarSlots[selectedSlotIndex];
        ItemInstance itemInstance = currentSlot.GetItem();

        if (itemInstance == null)
        {
            Debug.Log($"Slot {selectedSlotIndex + 1} kosong.");
            return;
        }
        if (itemInstance.itemData == null)
        {
            Debug.LogError("DATA CORRUPT: Item Instance ada, tapi ItemData null!");
            return;
        }

        ItemData itemData = itemInstance.itemData;

        switch (itemData.itemType)
        {
            case ItemType.Food:
                // Safety check playerStats
                if (playerStats != null) playerStats.EatFood(itemInstance.calculatedValue);
                currentSlot.ClearSlot();
                UpdatePlayerHand();
                break;

            case ItemType.Tool:
                // Safety check playerStats
                if (playerStats == null)
                {
                    Debug.LogError("PlayerStats belum di-assign di PlayerAction!");
                    return;
                }

                if (!playerStats.HasStamina())
                {
                    Debug.Log("Stamina habis.");
                    return;
                }

                if (playerVisual != null) playerVisual.PlayMiningAnimation();
                TryMineRock();
                break;

            case ItemType.Resource:
                Debug.Log($"Resource tidak bisa dipakai langsung.");
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
        if (distance > miningRange)
        {
            Debug.Log("Target terlalu jauh.");
            return;
        }

        rock.TakeDamage();
        if (playerStats != null) playerStats.ConsumeStaminaForMining();
    }
}