using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlatformEffector2D))]
[RequireComponent(typeof(Collider2D))]
public class Platform : MonoBehaviour
{
    private PlatformEffector2D parentPlatformEffector;
    void Start()
    {
        parentPlatformEffector = transform.parent.gameObject.GetComponent<PlatformEffector2D>();
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && parentPlatformEffector.rotationalOffset == 180f)
        {
            Debug.Log("Exited trigger zone of platform.");
            parentPlatformEffector.rotationalOffset = 0f;
        }
    }
}
