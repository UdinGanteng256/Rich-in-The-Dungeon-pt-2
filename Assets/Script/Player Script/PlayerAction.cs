using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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

    [Header("Mining Settings")]
    public LayerMask miningLayer;

    [SerializeField] private Animator animator;
    private Camera mainCamera;

    #region Unity Lifecycle
    void Start()
    {
        mainCamera = Camera.main;

        playerVisual ??= FindObjectOfType<PlayerVisual>();
        playerStats ??= FindObjectOfType<PlayerStats>();
        playerMovement ??= GetComponent<PlayerMovement>();
        playerBodyLocation ??= transform;

        InitializeHotbarSlots();
        SelectSlot(-1);
    }

    void Update()
    {
        HandleHotbarInput();
        HandleActionInput();
    }
    #endregion

    #region Input Handling
    void HandleHotbarInput()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) ToggleSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) ToggleSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) ToggleSlot(2);
    }

    void HandleActionInput()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        // Kalo ada raycast yang nutupin jadi ga bisa diklik
        if (EventSystem.current.IsPointerOverGameObject()) return;

        PerformAction();
    }
    #endregion

    #region Hotbar Logic
    void InitializeHotbarSlots()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i] != null)
                hotbarSlots[i].slotIndex = i;
        }
    }

    public void OnHotbarSlotClicked(int index)
    {
        ToggleSlot(index);
    }

    void ToggleSlot(int index)
    {
        if (index < 0 || index >= hotbarSlots.Length) return;
        SelectSlot(selectedSlotIndex == index ? -1 : index);
    }

    public void SelectSlot(int index)
    {
        selectedSlotIndex = index;

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i].SetHighlight(i == selectedSlotIndex);
        }

        UpdatePlayerHand();
        UpdateArmedState();
    }

    public ActiveSlot GetCurrentActiveSlot()
    {
        if (selectedSlotIndex < 0 || selectedSlotIndex >= hotbarSlots.Length)
            return null;

        return hotbarSlots[selectedSlotIndex];
    }
    #endregion

    #region Visual & Animation
    void UpdatePlayerHand()
    {
        if (playerVisual == null) return;

        ItemInstance item = selectedSlotIndex == -1
            ? null
            : hotbarSlots[selectedSlotIndex].GetItem();

        playerVisual.UpdateHandSprite(item?.itemData);
    }

    void UpdateArmedState()
    {
        if (animator == null) return; 

        bool isArmed =
            selectedSlotIndex != -1 &&
            hotbarSlots[selectedSlotIndex].GetItem()?.itemData?.itemType == ItemType.Tool;

        animator.SetFloat("isArmed", isArmed ? 1f : 0f);
    }

    public void ForceUpdateVisuals()
    {
        UpdatePlayerHand();
        UpdateArmedState();

        InventoryUI ui = FindObjectOfType<InventoryUI>();
        if (ui != null) ui.RefreshInventoryItems();
    }
    #endregion

    #region Action Logic
    void PerformAction()
    {
        ActiveSlot currentSlot = GetCurrentActiveSlot();
        if (currentSlot == null) return;

        ItemInstance itemInstance = currentSlot.GetItem();
        if (itemInstance?.itemData == null) return;

        switch (itemInstance.itemData.itemType)
        {
            case ItemType.Food:
                ConsumeFood(itemInstance, currentSlot);
                break;

            case ItemType.Tool:
                TryUseTool();
                break;
        }
    }

    void ConsumeFood(ItemInstance item, ActiveSlot slot)
    {
        if (playerStats == null) return;

        playerStats.EatFood(item.calculatedValue);
        slot.ClearSlot();
        ForceUpdateVisuals();
    }

    void TryUseTool()
    {
        if (playerStats == null || !playerStats.HasStamina()) return;

        TryMineRock();
    }
    #endregion

    #region Mining
    void TryMineRock()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, miningLayer);

        if (hit.collider == null) return;

        MiningNode rock = hit.collider.GetComponent<MiningNode>();
        if (rock == null) return;

        float distance = Vector2.Distance(playerBodyLocation.position, hit.point);
        if (distance > miningRange) return;

        rock.TakeDamage();
        playerStats.ConsumeStaminaForMining();
        animator?.SetTrigger("Mining");
    }
    #endregion
}
