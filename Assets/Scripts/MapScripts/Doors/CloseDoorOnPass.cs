using UnityEngine;

/// <summary>
/// Closes doors (animator 'isOpen'=false + enables blocking collider) when player passes through.
/// Can be attached directly to door GameObjects or to a separate trigger.
/// 
/// Usage:
/// - If on the door itself: ensure the door has a trigger Collider2D (will create if missing)
/// - Configure 'otherDoors' array for additional doors to close (e.g. paired/linked doors)
/// - Component closes self automatically if door has Animator; can also close "otherDoors"
/// </summary>
public class CloseDoorOnPass : MonoBehaviour
{
    [Tooltip("Additional doors to close (optional) — self is always closed")]
    public GameObject[] otherDoors;

    private const string PLAYER_TAG = "Player";
    private Collider2D triggerCollider;
    private bool initialized;

    private void Start()
    {
        InitializeTrigger();
    }

    private void InitializeTrigger()
    {
        if (initialized) return;
        initialized = true;

        // Look for a trigger collider on this object
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            if (col.isTrigger)
            {
                triggerCollider = col;
                break;
            }
        }

        // If no trigger found, create one
        if (triggerCollider == null)
        {
            var boxCol = gameObject.AddComponent<BoxCollider2D>();
            if (boxCol != null)
            {
                boxCol.isTrigger = true;
                boxCol.size = new Vector2(2f, 3f);
            }
            triggerCollider = boxCol;
            Debug.Log($"[CloseDoorOnPass] Created trigger collider on {gameObject.name}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!initialized) InitializeTrigger();
        if (!other.CompareTag(PLAYER_TAG)) return;

        // Close self
        CloseDoor(gameObject);

        // Close other doors if provided
        if (otherDoors != null)
        {
            foreach (var door in otherDoors)
            {
                if (door != null && door != gameObject)
                {
                    CloseDoor(door);
                }
            }
        }
    }

    private void CloseDoor(GameObject door)
    {
        // Set animator
        if (door.TryGetComponent<Animator>(out var animator))
        {
            animator.SetBool("isOpen", false);
        }

        // Enable first non-trigger collider (blocking collider)
        var colliders = door.GetComponents<Collider2D>();
        foreach (var c in colliders)
        {
            if (!c.isTrigger)
            {
                c.enabled = true;
                break;
            }
        }
    }
}

