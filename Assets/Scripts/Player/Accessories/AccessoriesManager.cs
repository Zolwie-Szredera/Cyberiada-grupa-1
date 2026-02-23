using UnityEngine;

public class AccessoriesManager : MonoBehaviour
{
    public Accessory currentHead;
    public Accessory currentBody;
    public Accessory currentArms;
    public Accessory currentLegs;
    //Sorry, but I can't bother with a proper scriptableObject database mumbo-jumbo, so here is a list of every accessorry in the game:
    public Accessory[] allHeadAccessories; //fill it up in the editor
    public Accessory[] allBodyAccessories;
    public Accessory[] allArmsAccessories;
    public Accessory[] allLegsAccessories;
    public Accessory defaultAccessory;
    public void Equip(Accessory accessory)
    {
        Debug.Log("Equipping accessory: " + accessory.name);
        accessory.Apply();
    }

    public void Unequip(Accessory accessory)
    {
        Debug.Log("Unequipping accessory: " + accessory.name);
        accessory.Remove();
    }
    public void Start()
    {
        Equip(currentLegs);
        Equip(currentArms);
        Equip(currentBody);
        Equip(currentHead);
    }
}
