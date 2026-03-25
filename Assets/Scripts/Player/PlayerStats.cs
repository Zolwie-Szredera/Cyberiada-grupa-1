using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseMaxBlood = 100f;
    public float baseMoveSpeed = 12f;
    public float baseJumpForce = 15f;
    public int baseAirJumps = 1;
    public int baseSwordDamage = 1;
    public int baseRangedDamage = 1;
    public float baseAttackSpeed = 1f;

    [Header("Current Stats")]
    [HideInInspector] public float maxBlood;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float jumpForce;
    [HideInInspector] public int airJumps;
    [HideInInspector] public int swordDamage;
    [HideInInspector] public int rangedDamage;
    [HideInInspector] public float attackSpeed;

    private readonly List<Accessory> activeAccessories = new();

    public delegate void OnStatsChangedDelegate();
    public event OnStatsChangedDelegate OnStatsChanged;

    private void Start()
    {
        InitializeStats();
        InitializeAccessoriesManager();
    }

    private void InitializeAccessoriesManager()
    {
        var accManager = GetComponent<AccessoriesManager>();
        if (accManager == null)
        {
            accManager = gameObject.AddComponent<AccessoriesManager>();
            accManager.playerStats = this;
        }
        
        var test = GetComponent<AccessoriesTest>();
        if (test == null)
        {
            test = gameObject.AddComponent<AccessoriesTest>();
            test.autoAddToPlayer = false;
            test.runTestOnStart = true;
        }
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
}
