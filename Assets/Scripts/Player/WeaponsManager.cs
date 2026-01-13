using UnityEngine;

/// <summary>
/// WEAPONS MANAGER - REWRITTEN
/// Manages multiple weapons and their orbit behavior
/// Simplified to work with new WeaponOrbit system
/// </summary>
public class WeaponsManager : MonoBehaviour
{
    [Header("Weapon References")]
    [Tooltip("All weapon GameObjects")]
    public GameObject[] weapons;
    
    [Header("Orbit Configuration")]
    [Tooltip("Distance weapons orbit from player")]
    public float orbitRadius = 1.5f;
    
    [Tooltip("Smoothness of weapon movement (0 = instant, higher = smoother)")]
    [Range(0f, 30f)]
    public float positionSmoothSpeed = 10f;
    
    [Tooltip("Rotation offset for all weapons (adjust if sprites point wrong way)")]
    public float globalRotationOffset;
    
    [Header("Sprite Flipping")]
    [Tooltip("Auto-flip weapon sprites when aiming left")]
    public bool enableSpriteFlipping = true;
    
    [Header("Debug")]
    [Tooltip("Add visual debug arrows to show weapon facing")]
    public bool addDebugArrows = true;
    
    [Tooltip("Draw orbit circles in scene view")]
    public bool drawOrbitGizmos = true;

    private WeaponOrbit[] weaponOrbits;
    private int currentWeaponIndex;

    void Start()
    {
        InitializeWeaponOrbits();
        
        // Show only first weapon
        if (weapons.Length > 0)
        {
            SwitchToWeapon(0);
        }
    }

    void InitializeWeaponOrbits()
    {
        if (weapons == null || weapons.Length == 0)
        {
            Debug.LogWarning("[WeaponsManager] No weapons assigned!");
            return;
        }

        weaponOrbits = new WeaponOrbit[weapons.Length];

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
            {
                Debug.LogWarning($"[WeaponsManager] Weapon slot {i} is null!");
                continue;
            }

            // Get or add WeaponOrbit component
            weaponOrbits[i] = weapons[i].GetComponent<WeaponOrbit>();
            if (weaponOrbits[i] == null)
            {
                weaponOrbits[i] = weapons[i].AddComponent<WeaponOrbit>();
            }

            // Configure orbit
            weaponOrbits[i].orbitRadius = orbitRadius;
            weaponOrbits[i].positionSmoothSpeed = positionSmoothSpeed;
            weaponOrbits[i].rotationOffset = globalRotationOffset;
            weaponOrbits[i].enableSpriteFlipping = enableSpriteFlipping;

            // Add debug arrow if enabled
            if (addDebugArrows)
            {
                AddDebugArrow(weapons[i]);
            }

            Debug.Log($"[WeaponsManager] Initialized weapon {i}: {weapons[i].name}");
        }
    }

    void AddDebugArrow(GameObject weapon)
    {
        // Check if already has arrow
        WeaponDirectionIndicator existing = weapon.GetComponentInChildren<WeaponDirectionIndicator>();
        if (existing != null)
            return;

        // Create arrow child object
        GameObject arrowObj = new GameObject("DebugArrow");
        arrowObj.transform.SetParent(weapon.transform);
        arrowObj.transform.localPosition = Vector3.zero;
        arrowObj.transform.localRotation = Quaternion.identity;

        // Add indicator
        WeaponDirectionIndicator arrow = arrowObj.AddComponent<WeaponDirectionIndicator>();
        arrow.arrowLength = 2.0f;
        arrow.arrowColor = Color.red;
        arrow.showInGame = true;
    }

    public void SwitchToWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length)
        {
            Debug.LogWarning($"[WeaponsManager] Invalid weapon index: {index}");
            return;
        }

        currentWeaponIndex = index;

        // Hide all weapons
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].SetActive(i == index);
            }
        }

        Debug.Log($"[WeaponsManager] Switched to weapon: {weapons[index].name}");
    }

    public int GetCurrentWeaponIndex()
    {
        return currentWeaponIndex;
    }

    public GameObject GetCurrentWeapon()
    {
        if (currentWeaponIndex >= 0 && currentWeaponIndex < weapons.Length)
        {
            return weapons[currentWeaponIndex];
        }
        return null;
    }

    // Update all weapon orbit settings at runtime
    public void UpdateOrbitSettings(float radius, float smoothSpeed)
    {
        orbitRadius = radius;
        positionSmoothSpeed = smoothSpeed;

        foreach (var orbit in weaponOrbits)
        {
            if (orbit != null)
            {
                orbit.orbitRadius = radius;
                orbit.positionSmoothSpeed = smoothSpeed;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!drawOrbitGizmos)
            return;

        // Draw orbit circle around player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f); // Transparent yellow
            DrawCircle(player.transform.position, orbitRadius, 48);
        }
    }

    void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0f
            );
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}

