using UnityEngine;

public class CloseDoor : Action
{
    public GameObject[] doors;
    public override void ExecuteAction()
    {
        foreach (GameObject door in doors)
        {
            Close(door);
            Debug.Log("closed door via:" + name);
        }
    }

    public override void UndoAction()
    {
        foreach (GameObject door in doors)
        {
            Open(door);
            Debug.Log("opened door via:" + name);
        }
    }
    public void Open(GameObject door)
    {
        if (!door.TryGetComponent<Animator>(out var animator))
        {
            Debug.LogError("animator not found in door");
        }
        animator.SetBool("isOpen", true);
    }
    public void Close(GameObject door)
    {
        if (!door.TryGetComponent<Animator>(out var animator))
        {
            Debug.LogError("animator not found in door");
        }
        animator.SetBool("isOpen", false);
    }
}
