using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponsManager : MonoBehaviour
{
    public GameObject[] weapons;
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
