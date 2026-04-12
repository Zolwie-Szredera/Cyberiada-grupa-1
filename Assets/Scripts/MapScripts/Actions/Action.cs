using UnityEngine;

public abstract class Action : MonoBehaviour
{
    public abstract void ExecuteAction();
    public abstract void UndoAction();
}
