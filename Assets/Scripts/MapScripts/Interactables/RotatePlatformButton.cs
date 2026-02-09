using UnityEngine;

public class RotatePlatformButton : Button
{
    public GameObject platform;
    public int angle;
    public override void Interaction()
    {
        platform.transform.Rotate(0,0,angle);
        Debug.Log("Interaction!");
    }
}
