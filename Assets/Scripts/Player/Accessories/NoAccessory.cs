using UnityEngine;
[CreateAssetMenu(menuName = "Accessories/NoAccessory")]
public class NoAccessory : Accessory
{
    public override void Apply()
    {
        // Do nothing - this is the default accessory
    }

    public override void Remove()
    {
        // Do nothing - this is the default accessory
    }
}
