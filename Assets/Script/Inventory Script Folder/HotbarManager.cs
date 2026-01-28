using UnityEngine;

public class HotbarManager : MonoBehaviour
{
    [Header("Masukkan Semua Slot Hotbar Disini")]
    public ActiveSlot[] hotbarSlots; 

    void Start()
    {
        if (GlobalData.hasDataInitialized)
        {
            if (GlobalData.savedHotbarItems != null && GlobalData.savedHotbarItems.Length == hotbarSlots.Length)
            {
                for (int i = 0; i < hotbarSlots.Length; i++)
                {
                    hotbarSlots[i].SetItemSilent(GlobalData.savedHotbarItems[i]);
                }
                Debug.Log("Hotbar Loaded!");
            }
        }
    }

    public void SaveHotbar()
    {
        if (GlobalData.savedHotbarItems == null || GlobalData.savedHotbarItems.Length != hotbarSlots.Length)
        {
            GlobalData.savedHotbarItems = new ItemInstance[hotbarSlots.Length];
        }

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            GlobalData.savedHotbarItems[i] = hotbarSlots[i].GetItem();
        }

        GlobalData.hasDataInitialized = true; 
    }
}