using UnityEngine;

public class DestroyObjects : Action
{
    //simple
    public GameObject[] gameObjects;
    public override void ExecuteAction()
    {
        foreach(GameObject gameObject in gameObjects)
        {
            Destroy(gameObject);
        }
    }

    public override void UndoAction()
    {
        throw new System.NotImplementedException();
    }
}
