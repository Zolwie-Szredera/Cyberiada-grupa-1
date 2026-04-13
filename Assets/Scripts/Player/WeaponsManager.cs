using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    [Header("Weapon References")]
    [Tooltip("Slot 0: Left Mouse Button (Melee)\nSlot 1: Right Mouse Button (Ranged)")]
    public GameObject[] weapons = new GameObject[2];

    [Header("Orbit Configuration")]
    [Tooltip("Distance weapons orbit from player")]
    public float orbitRadius = 1.5f;

    [Tooltip("How fast weapons follow mouse around orbit (0 = instant, higher = smoother)")]
    [Range(0f, 30f)]
    public float angleFollowSpeed = 15f;

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

    void Start()
    {
        InitializeWeaponOrbits();
        ActivateAssignedWeapons();
    }

    void InitializeWeaponOrbits()
    {
        if (weapons == null || weapons.Length < 2)
        {
            Debug.LogWarning("[WeaponsManager] Requires 2 weapon slots (LMB and RMB). Currently has: " + (weapons?.Length ?? 0));
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

            weaponOrbits[i] = weapons[i].GetComponent<WeaponOrbit>() ?? weapons[i].AddComponent<WeaponOrbit>();
            weaponOrbits[i].orbitRadius = orbitRadius;
            weaponOrbits[i].angleFollowSpeed = angleFollowSpeed;
            weaponOrbits[i].rotationOffset = globalRotationOffset;
            weaponOrbits[i].enableSpriteFlipping = enableSpriteFlipping;

            if (addDebugArrows)
            {
                AddDebugArrow(weapons[i]);
            }
        }
    }

    void ActivateAssignedWeapons()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].SetActive(true);
            }
        }
    }

    public GameObject GetWeaponAt(int index)
    {
        if (index < 0 || index >= weapons.Length)
        {
            return null;
        }

        return weapons[index];
    }

    void AddDebugArrow(GameObject weapon)
    {
        WeaponDirectionIndicator existing = weapon.GetComponentInChildren<WeaponDirectionIndicator>();
        if (existing != null)
            return;

        GameObject arrowObj = new("DebugArrow");
        arrowObj.transform.SetParent(weapon.transform);
        arrowObj.transform.localPosition = Vector3.zero;
        arrowObj.transform.localRotation = Quaternion.identity;

        WeaponDirectionIndicator arrow = arrowObj.AddComponent<WeaponDirectionIndicator>();
        arrow.arrowLength = 2.0f;
        arrow.arrowColor = Color.red;
        arrow.showInGame = true;
    }

    public void UpdateOrbitSettings(float radius, float followSpeed)
    {
        orbitRadius = radius;
        angleFollowSpeed = followSpeed;

        if (weaponOrbits == null)
            return;

        foreach (var orbit in weaponOrbits)
        {
            if (orbit != null)
            {
                orbit.orbitRadius = radius;
                orbit.angleFollowSpeed = followSpeed;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!drawOrbitGizmos)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
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
