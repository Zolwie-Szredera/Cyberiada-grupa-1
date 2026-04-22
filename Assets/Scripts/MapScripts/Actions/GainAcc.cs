using UnityEngine;

public class GainAcc : Action
{
    //gain access to a new accessory.
    //implement this when accessory system works, for now it's just a placeholder.
    public Accessory accessory;
    public DialogueData dialogueData;
    private AccessoriesManager accessoriesManager;
    private DialogueHandler dialogueHandler;
    private void Start()
    {
        accessoriesManager = GameObject.FindGameObjectWithTag("Player").GetComponent<AccessoriesManager>();
        dialogueHandler = GameObject.FindGameObjectWithTag("GameManager").GetComponentInChildren<DialogueHandler>();
        if(accessoriesManager == null || dialogueHandler == null)
        {
            Debug.LogError(">0 components not found");
        }
    }
    //this must be changed when we add a full system with UI
    public override void ExecuteAction()
    {
        accessoriesManager.AddToInventory(accessory);
        accessoriesManager.Equip(0,0);
        if(dialogueData != null)
        {
            //something like "Accessory found! It does blah blah blah"
            dialogueHandler.StartDialogue(dialogueData);
        }
    }
    //irrelevant.
    public override void UndoAction()
    {
        Debug.LogWarning("You shouldn't see this");
    }
}
