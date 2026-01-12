using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponsManager : MonoBehaviour
{
    public GameObject[] weapons;
    
    [Header("Orbit Settings")]
    [Tooltip("Use orbit system for weapons")]
    public bool useOrbitSystem = true;
    
    [Tooltip("Orbit radius around the player")]
    public float orbitRadius = 1.5f;
    
    [Tooltip("Weapon position adjustment speed (0 = instant, 10 = smooth)")]
    [Range(0f, 20f)]
    public float positionSmoothSpeed = 10f;

    private WeaponOrbit[] weaponOrbitScripts;
    
    void Start()
    {
        if (useOrbitSystem)
        {
            SetupWeaponOrbits();
        }
    }

    void SetupWeaponOrbits()
    {
        weaponOrbitScripts = new WeaponOrbit[weapons.Length];
        
        // Dodaj lub zaktualizuj komponenty WeaponOrbit
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                // Sprawdź czy broń już ma komponent WeaponOrbit
                weaponOrbitScripts[i] = weapons[i].GetComponent<WeaponOrbit>();
                
                if (weaponOrbitScripts[i] == null)
                {
                    // Dodaj komponent jeśli nie istnieje
                    weaponOrbitScripts[i] = weapons[i].AddComponent<WeaponOrbit>();
                }
                
                // Ustaw parametry orbity - wszystkie bronie będą podążać za myszą
                weaponOrbitScripts[i].orbitRadius = orbitRadius;
                weaponOrbitScripts[i].followMousePosition = true;
                weaponOrbitScripts[i].positionSmoothSpeed = positionSmoothSpeed;
                weaponOrbitScripts[i].rotateTowardsMouse = true;
            }
        }
    }
    
    //this is awful. Too bad, as long as it works its ok
    //I made it that way because input editor is kinda complicated
    public void OnSelectWeapon1(InputAction.CallbackContext context) //sword
    {
        if(!context.started) return;
        Debug.Log("Selected weapon 1");
        weapons[0].SetActive(true);
        weapons[1].SetActive(false);
        weapons[2].SetActive(false);
        weapons[3].SetActive(false);
    }
        public void OnSelectWeapon2(InputAction.CallbackContext context) //projectile shoot
    {
        if(!context.started) return;
        Debug.Log("Selected weapon 2");
        weapons[0].SetActive(false);
        weapons[1].SetActive(true);
        weapons[2].SetActive(false);
        weapons[3].SetActive(false);
    }
        public void OnSelectWeapon3(InputAction.CallbackContext context) //WIP
    {
        if(!context.started) return;
        Debug.Log("Selected weapon 3");
        weapons[0].SetActive(false);
        weapons[1].SetActive(false);
        weapons[2].SetActive(true);
        weapons[3].SetActive(false);
    }
        public void OnSelectWeapon4(InputAction.CallbackContext context) //WIP
    {
        if(!context.started) return;
        Debug.Log("Selected weapon 4");
        weapons[0].SetActive(false);
        weapons[1].SetActive(false);
        weapons[2].SetActive(false);
        weapons[3].SetActive(true);
    }
}
