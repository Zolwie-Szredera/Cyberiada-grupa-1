using UnityEngine;

public class RotatePlatformButton : Button
{
    public GameObject platform;
    public override void Interaction()
    {
        platform.transform.Rotate(0,0,5);
        Debug.Log("Interaction!");
    }
}
