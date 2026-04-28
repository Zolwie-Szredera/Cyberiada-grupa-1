using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AccessoriesManager))]
public class PlayerStats : MonoBehaviour
{
    [Header("Movement")] //rememer to use the stats without "base" in name during runtime
    public float baseMoveSpeed = 12f; //for: PlayerController.cs
    public float baseJumpForce = 15f; //for: PlayerController.cs
    public int baseAirJumps = 1; //for: PlayerController.cs
    public float baseAccelerationRate = 30f; //for: PlayerController.cs
    public float baseDecelerationRate = 100f; //for: PlayerController.cs
    public float baseDashForce = 25f; //for: Dash.cs
    [Header("Health")]
    public float baseMaxBlood = 30f; //for: playerHealth.cs
    [Header("weapons")]
    public int baseSwordDamage = 10; //for: PlayerSword.cs
    public int baseRangedDamage = 20; //for: PlayerRanged.cs
    public float baseAttackSpeed = 1f; //for PlayerRanged.cs

    [Header("Current Stats")]
    [HideInInspector] public static float maxBlood;
    [HideInInspector] public static float moveSpeed;
    [HideInInspector] public static float jumpForce;
    [HideInInspector] public static int airJumps;
    [HideInInspector] public static int swordDamage;
    [HideInInspector] public static int rangedDamage;
    [HideInInspector] public static float attackSpeed;
    [HideInInspector] public static float accelerationRate;
    [HideInInspector] public static float decelerationRate;
    [HideInInspector] public static float dashForce;

    private readonly List<Accessory> activeAccessories = new();

    public delegate void OnStatsChangedDelegate();
    public event OnStatsChangedDelegate OnStatsChanged;
    
    private void Awake()
    {
        InitializeStats();
        //CommenceAccessoryTest(); // uncomment when you want to use AccessoriesTest.cs
    }

    public void InitializeStats()
    {
        maxBlood = baseMaxBlood;
        moveSpeed = baseMoveSpeed;
        jumpForce = baseJumpForce;
        airJumps = baseAirJumps;
        swordDamage = baseSwordDamage;
        rangedDamage = baseRangedDamage;
        attackSpeed = baseAttackSpeed;
        accelerationRate = baseAccelerationRate;
        decelerationRate = baseDecelerationRate;
        dashForce = baseDashForce;
        OnStatsChanged?.Invoke();
    }

    public void AddAccessory(Accessory accessory)
    {
        if (accessory == null) return;
        
        activeAccessories.Add(accessory);
        accessory.Apply(this);
        OnStatsChanged?.Invoke();
        Debug.Log($"[PlayerStats] Added accessory: {accessory.name}");
    }

    public void RemoveAccessory(Accessory accessory)
    {
        if (accessory == null) return;
        
        if (activeAccessories.Remove(accessory))
        {
            accessory.Remove(this);
            OnStatsChanged?.Invoke();
            Debug.Log($"[PlayerStats] Removed accessory: {accessory.name}");
        }
    }

    public IReadOnlyList<Accessory> GetActiveAccessories()
    {
        return activeAccessories.AsReadOnly();
    }
    //debug
    public void CommenceAccessoryTest()
    {
        var test = GetComponent<AccessoriesTest>();
        if (test == null)
        {
            test = gameObject.AddComponent<AccessoriesTest>();
            test.autoAddToPlayer = false;
            test.runTestOnStart = true;
        }
    }
}
