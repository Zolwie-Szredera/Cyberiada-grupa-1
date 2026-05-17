using UnityEngine;

/// <summary>
/// Small helper: when player passes through trigger, close given doors by setting Animator 'isOpen' to false
/// and enabling any non-trigger Collider2D on the door GameObjects.
/// This uses the same convention as your existing OpenDoor/CloseDoor actions (animator parameter 'isOpen').
/// Attach to a trigger Collider2D (isTrigger=true) placed at the door passage.
/// </summary>
public class CloseDoorOnPass : MonoBehaviour
{
    public GameObject[] doors;
    private const string PLAYER_TAG = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(PLAYER_TAG)) return;

        foreach (var door in doors)
        {
            if (door == null) continue;

            // animator
            if (door.TryGetComponent<Animator>(out var animator))
            {
                animator.SetBool("isOpen", false);
            }

            // enable collider to physically block (first non-trigger collider found)
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
}

