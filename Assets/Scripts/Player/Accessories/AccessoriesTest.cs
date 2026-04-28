using UnityEngine;

public class AccessoriesTest : MonoBehaviour
{
    [Header("Test Settings")]
    [Tooltip("Set to true to run test automatically on start")]
    public bool runTestOnStart = true;

    [Tooltip("Set to true to auto-add this component to player if not present")]
    public bool autoAddToPlayer = true;

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

        var charm = ScriptableObject.CreateInstance<LilyOfShadows>();
        Debug.Log($"Created accessory: {charm.name}");

        Debug.Log("\n--- Adding Lily of Shadows to inventory ---");
        bool added = accManager.AddToInventory(charm);
        Debug.Log($"AddToInventory result: {added}");

        Debug.Log("\n--- Equipping to slot 0 ---");
        bool equipped = accManager.Equip(0, 0);
        Debug.Log($"Equip result: {equipped}");

        Debug.Log("\n--- Testing Flayed Skin ===");
        var flayedSkin = ScriptableObject.CreateInstance<FlayedSkin>();
        Debug.Log($"Created accessory: {flayedSkin.name}");

        Debug.Log("\n--- Adding Flayed Skin to inventory ---");
        bool addedFlayed = accManager.AddToInventory(flayedSkin);
        Debug.Log($"AddToInventory result: {addedFlayed}");

        Debug.Log("\n--- Equipping Flayed Skin to slot 1 ---");
        bool equippedFlayed = accManager.Equip(0, 1);
        Debug.Log($"Equip result: {equippedFlayed}");

        if (TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            Debug.Log($"Flayed Skin protection active: {playerHealth.preventLethalBloodSpellCosts}");
        }

        Debug.Log("\n=== Accessories System Test Completed ===");
    }

    public void TestAddAccessory()
    {
        if (accManager == null) accManager = GetComponent<AccessoriesManager>();
        if (playerStats == null) playerStats = GetComponent<PlayerStats>();
        
        var charm = ScriptableObject.CreateInstance<LilyOfShadows>();
        accManager.AddToInventory(charm);
        accManager.Equip(0, 0);

        var flayed = ScriptableObject.CreateInstance<FlayedSkin>();
        accManager.AddToInventory(flayed);
        accManager.Equip(1, 1);
    }

    public void TestRemoveAccessory()
    {
        if (accManager == null) accManager = GetComponent<AccessoriesManager>();
        accManager.Unequip(0);
    }
}
