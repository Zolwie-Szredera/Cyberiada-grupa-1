using UnityEngine;

public class AccessoriesTest : MonoBehaviour
{
    [Header("Test Settings")]
    [Tooltip("Set to true to run test automatically on start")]
    public bool runTestOnStart = true;

    [Tooltip("Set to true to auto-add this component to player if not present")]
    public bool autoAddToPlayer = true;

    [Header("Test Accessory")]
    public Accessory testAccessory;

    private AccessoriesManager accManager;
    private PlayerStats playerStats;
    private static bool wasInitialized = false;

    private void Awake()
    {
        if (autoAddToPlayer && !wasInitialized)
        {
            wasInitialized = true;
            gameObject.AddComponent<AccessoriesTest>();
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        accManager = GetComponent<AccessoriesManager>();
        playerStats = GetComponent<PlayerStats>();

        if (runTestOnStart && accManager != null && playerStats != null)
        {
            RunTest();
        }
    }

    private void RunTest()
    {
        Debug.Log("=== Accessories System Test Started ===");
        Debug.Log($"Initial Stats - MaxBlood: {PlayerStats.maxBlood}, SwordDamage: {PlayerStats.swordDamage}");

        if (testAccessory == null)
        {
            Debug.LogWarning("AccessoriesTest: No testAccessory assigned.");
            return;
        }

        Debug.Log($"Using accessory: {testAccessory.Name}");

        Debug.Log("\n--- Adding to inventory ---");
        bool added = accManager.AddToInventory(testAccessory);
        Debug.Log($"AddToInventory result: {added}");

        Debug.Log("\n--- Equipping to slot 0 ---");
        bool equipped = accManager.Equip(0, 0);
        Debug.Log($"Equip result: {equipped}");
        Debug.Log($"After Equip - MaxBlood: {PlayerStats.maxBlood}, SwordDamage: {PlayerStats.swordDamage}");

        Debug.Log("\n--- Unequipping from slot 0 ---");
        bool unequipped = accManager.Unequip(0);
        Debug.Log($"Unequip result: {unequipped}");
        Debug.Log($"After Unequip - MaxBlood: {PlayerStats.maxBlood}, SwordDamage: {PlayerStats.swordDamage}");

        Debug.Log("\n=== Accessories System Test Completed ===");
    }

    public void TestAddAccessory()
    {
        if (accManager == null) accManager = GetComponent<AccessoriesManager>();
        if (playerStats == null) playerStats = GetComponent<PlayerStats>();
        
        if (testAccessory == null)
        {
            Debug.LogWarning("AccessoriesTest: No testAccessory assigned.");
            return;
        }

        accManager.AddToInventory(testAccessory);
        accManager.Equip(0, 0);
    }

    public void TestRemoveAccessory()
    {
        if (accManager == null) accManager = GetComponent<AccessoriesManager>();
        accManager.Unequip(0);
    }
}
