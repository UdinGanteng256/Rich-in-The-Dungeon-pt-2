using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    [Header("Hotbar")]
    public ActiveSlot[] hotbarSlots;

    // -1 berarti tidak ada slot yang aktif
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

        if (playerVisual == null)
            playerVisual = FindObjectOfType<PlayerVisual>();

        if (playerBodyLocation == null)
            playerBodyLocation = transform;

        // Mulai dengan tangan kosong
        SelectSlot(-1);
    }

    void Update()
    {
        // Hotbar input (toggle)
        if (Keyboard.current.digit1Key.wasPressedThisFrame) ToggleSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) ToggleSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) ToggleSlot(2);

        if (Mouse.current.leftButton.wasPressedThisFrame)
            PerformAction();
    }

    void ToggleSlot(int index)
    {
        if (index < 0 || index >= hotbarSlots.Length) return;

        // Tekan slot yang sama = unequip
        SelectSlot(selectedSlotIndex == index ? -1 : index);
    }

    void SelectSlot(int index)
    {
        selectedSlotIndex = index;

        // Update highlight UI
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i].SetHighlight(i == selectedSlotIndex);
        }

        UpdatePlayerHand();
    }

    void UpdatePlayerHand()
    {
        if (playerVisual == null) return;

        // Tangan kosong jika tidak ada slot aktif
        if (selectedSlotIndex == -1)
        {
            playerVisual.UpdateHandSprite(null);
            return;
        }

        ActiveSlot currentSlot = hotbarSlots[selectedSlotIndex];
        playerVisual.UpdateHandSprite(currentSlot.currentItem);
    }

    void PerformAction()
    {
        if (selectedSlotIndex == -1)
        {
            Debug.Log("Tidak ada item di tangan.");
            return;
        }

        ActiveSlot currentSlot = hotbarSlots[selectedSlotIndex];

        if (currentSlot.currentItem == null)
        {
            Debug.Log($"Slot {selectedSlotIndex + 1} kosong.");
            return;
        }

        ItemData item = currentSlot.currentItem;

        switch (item.itemType)
        {
            case ItemType.Food:
                playerStats.EatFood(item.valueAmount);
                currentSlot.ClearSlot();
                UpdatePlayerHand();
                break;

            case ItemType.Tool:
                if (!playerStats.HasStamina())
                {
                    Debug.Log("Stamina habis.");
                    return;
                }

                if (playerVisual != null)
                    playerVisual.PlayMiningAnimation();

                TryMineRock();
                break;

            case ItemType.Resource:
                Debug.Log("Item resource tidak bisa digunakan.");
                break;
        }
    }

    void TryMineRock()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider == null) return;

        MiningNode rock = hit.collider.GetComponent<MiningNode>();
        if (rock == null) return;

        float distance = Vector2.Distance(
            playerBodyLocation.position,
            hit.point
        );

        if (distance > miningRange)
        {
            Debug.Log("Target terlalu jauh.");
            return;
        }

        rock.TakeDamage();
        playerStats.ConsumeStaminaForMining();
    }

    void OnDrawGizmosSelected()
    {
        if (playerBodyLocation == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            playerBodyLocation.position,
            miningRange
        );
    }
}
