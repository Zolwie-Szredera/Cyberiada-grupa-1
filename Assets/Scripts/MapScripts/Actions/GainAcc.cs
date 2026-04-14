using UnityEngine;

public enum AccessoryType
{
    LilyOfShadows
}

[CreateAssetMenu(fileName = "NewGainAccReward", menuName = "Rewards/Gain Accessory")]
public class GainAcc : ScriptableObject
{
    [Tooltip("Wybierz akcesorium do przyznania")]
    public AccessoryType accessoryToGain = AccessoryType.LilyOfShadows;

    [Min(1)]
    public int quantity = 1;

    [Tooltip("Jeśli włączone, dodane akcesorium spróbuje od razu trafić do aktywnego slota")]
    public bool autoEquip;

    public void GainAccessory()
    {
        AccessoriesManager accessoriesManager = GameObject.FindGameObjectWithTag("Player")?.GetComponent<AccessoriesManager>();

        if (accessoriesManager == null)
        {
            Debug.LogError("[GainAcc] AccessoriesManager not found on Player!");
            return;
        }

        for (int i = 0; i < quantity; i++)
        {
            Accessory accessory = CreateAccessoryFromType(accessoryToGain);
            if (accessory == null)
            {
                Debug.LogError($"[GainAcc] Could not create accessory: {accessoryToGain}");
                return;
            }

            if (!accessoriesManager.AddToInventory(accessory))
            {
                Debug.LogWarning($"[GainAcc] Inventory full! Could not add all {quantity} of {accessoryToGain}");
                return;
            }

            if (autoEquip && TryAutoEquip(accessoriesManager, accessory))
            {
                Debug.Log($"[GainAcc] Auto-equipped {accessoryToGain}");
            }
        }

        Debug.Log($"[GainAcc] Added {quantity}x {accessoryToGain} to inventory");
    }

    private bool TryAutoEquip(AccessoriesManager accessoriesManager, Accessory accessory)
    {
        int inventoryIndex = FindInventoryIndex(accessoriesManager, accessory);
        if (inventoryIndex < 0)
        {
            Debug.LogWarning($"[GainAcc] Could not find inventory slot for {accessory.name}");
            return false;
        }

        int activeSlotIndex = FindFreeActiveSlot(accessoriesManager);
        if (activeSlotIndex < 0)
        {
            Debug.LogWarning($"[GainAcc] No free active slot for {accessory.name}");
            return false;
        }

        return accessoriesManager.Equip(inventoryIndex, activeSlotIndex);
    }

    private int FindInventoryIndex(AccessoriesManager accessoriesManager, Accessory accessory)
    {
        for (int i = 0; i < AccessoriesManager.InventorySlotsCount; i++)
        {
            if (accessoriesManager.GetInventorySlot(i) == accessory)
            {
                return i;
            }
        }

        return -1;
    }

    private int FindFreeActiveSlot(AccessoriesManager accessoriesManager)
    {
        for (int i = 0; i < AccessoriesManager.ActiveSlotsCount; i++)
        {
            if (accessoriesManager.GetActiveSlot(i) == null)
            {
                return i;
            }
        }

        return -1;
    }

    private Accessory CreateAccessoryFromType(AccessoryType type)
    {
        return type switch
        {
            AccessoryType.LilyOfShadows => new LilyOfShadows(),
            _ => null
        };
    }
}
