using UnityEngine;

public class ExecuteOnGameStart : MonoBehaviour
{
    public Action[] actionToExecute;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Action action in actionToExecute)
        {
            action.ExecuteAction();
        }
    }
    //I don't think that we ever need to undo the actions executed on game start
}
