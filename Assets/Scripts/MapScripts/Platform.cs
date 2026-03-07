using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Platform : MonoBehaviour
{
    private PlatformEffector2D parentPlatformEffector;
    private int disabledPlatformOffset = 180;
    void Start()
    {
        parentPlatformEffector = transform.parent.gameObject.GetComponent<PlatformEffector2D>();
        if (parentPlatformEffector == null)
        {
            Debug.LogError("Platform script error: No PlatformEffector2D found on parent of " + gameObject.name);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && parentPlatformEffector.rotationalOffset == disabledPlatformOffset)
        {
            parentPlatformEffector.rotationalOffset = 0f;
        }
    }
    public void RemoveCollision()
    {
        if (parentPlatformEffector.rotationalOffset != disabledPlatformOffset)
        {
            parentPlatformEffector.rotationalOffset = disabledPlatformOffset;
        }
    }
}
