using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class AccessoriesManager : MonoBehaviour
{
    private PlayerStats playerStats;

    public const int ActiveSlotsCount = 4;
    public const int InventorySlotsCount = 16;

    public Accessory[] activeSlots = new Accessory[ActiveSlotsCount];
    public Accessory[] inventory = new Accessory[InventorySlotsCount];
    public void Start() //please stop removing this. Seriously, why!?
    {
        playerStats = GetComponent<PlayerStats>();
    }
    public bool AddToInventory(Accessory accessory)
    {
        if (accessory == null) return false;

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = accessory;
                Debug.Log($"[AccessoriesManager] Added {accessory.Name} to inventory slot {i}");
                return true;
            }
        }

        Debug.LogWarning($"[AccessoriesManager] Inventory full! Cannot add {accessory.Name}");
        return false;
    }

    public bool RemoveFromInventory(int inventoryIndex)
    {
        if (inventoryIndex < 0 || inventoryIndex >= inventory.Length)
        {
            Debug.LogWarning($"[AccessoriesManager] Invalid inventory index: {inventoryIndex}");
            return false;
        }

        if (inventory[inventoryIndex] == null)
        {
            Debug.LogWarning($"[AccessoriesManager] Inventory slot {inventoryIndex} is already empty");
            return false;
        }

        inventory[inventoryIndex] = null;
        Debug.Log($"[AccessoriesManager] Removed accessory from inventory slot {inventoryIndex}");
        return true;
    }

    public bool Equip(int inventoryIndex, int activeSlotIndex)
    {
        if (inventoryIndex < 0 || inventoryIndex >= inventory.Length)
        {
            Debug.LogWarning($"[AccessoriesManager] Invalid inventory index: {inventoryIndex}");
            return false;
        }

        if (activeSlotIndex < 0 || activeSlotIndex >= activeSlots.Length)
        {
            Debug.LogWarning($"[AccessoriesManager] Invalid active slot index: {activeSlotIndex}");
            return false;
        }

        Accessory accessory = inventory[inventoryIndex];
        if (accessory == null)
        {
            Debug.LogWarning($"[AccessoriesManager] No accessory in inventory slot {inventoryIndex}");
            return false;
        }

        Accessory previousAccessory = activeSlots[activeSlotIndex];
        if (previousAccessory != null)
        {
            playerStats.RemoveAccessory(previousAccessory);
            inventory[inventoryIndex] = previousAccessory;
        }
        else
        {
            inventory[inventoryIndex] = null;
        }

        activeSlots[activeSlotIndex] = accessory;
        playerStats.AddAccessory(accessory);
        Debug.Log($"[AccessoriesManager] Equipped {accessory.Name} to active slot {activeSlotIndex}");
        return true;
    }

    public bool Unequip(int activeSlotIndex)
    {
        if (activeSlotIndex < 0 || activeSlotIndex >= activeSlots.Length)
        {
            Debug.LogWarning($"[AccessoriesManager] Invalid active slot index: {activeSlotIndex}");
            return false;
        }

        Accessory accessory = activeSlots[activeSlotIndex];
        if (accessory == null)
        {
            Debug.LogWarning($"[AccessoriesManager] No accessory in active slot {activeSlotIndex}");
            return false;
        }

        int freeInventorySlot = FindFreeInventorySlot();
        if (freeInventorySlot < 0)
        {
            Debug.LogWarning($"[AccessoriesManager] Cannot unequip - inventory full!");
            return false;
        }

        playerStats.RemoveAccessory(accessory);
        activeSlots[activeSlotIndex] = null;
        inventory[freeInventorySlot] = accessory;
        Debug.Log($"[AccessoriesManager] Unequipped {accessory.Name} from slot {activeSlotIndex} to inventory slot {freeInventorySlot}");
        return true;
    }

    public bool SwapInventorySlots(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= inventory.Length || indexB < 0 || indexB >= inventory.Length)
        {
            Debug.LogWarning($"[AccessoriesManager] Invalid inventory indices: {indexA}, {indexB}");
            return false;
        }

        (inventory[indexA], inventory[indexB]) = (inventory[indexB], inventory[indexA]);
        Debug.Log($"[AccessoriesManager] Swapped inventory slots {indexA} and {indexB}");
        return true;
    }

    private int FindFreeInventorySlot()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    public Accessory GetActiveSlot(int index)
    {
        if (index >= 0 && index < activeSlots.Length)
            return activeSlots[index];
        return null;
    }

    public Accessory GetInventorySlot(int index)
    {
        if (index >= 0 && index < inventory.Length)
            return inventory[index];
        return null;
    }
}
