using UnityEngine;

public class DialogueButton : Button
{
    public DialogueData currentData;
    private DialogueHandler dialogueHandler;
    public void Start()
    {
        dialogueHandler = GameObject.FindGameObjectWithTag("GameManager").GetComponentInChildren<DialogueHandler>();
        if (dialogueHandler == null)
        {
            Debug.LogError("DialogueHandler not found in children of GameManager! Please make sure you have a DialogueHandler script on a child of GameManager and that it has the tag 'GameManager'!");
        }
    }
    public override void Interaction()
    {
        if(dialogueHandler.isOpen) return; //prevent starting a new dialogue while one is already active.
        dialogueHandler.StartDialogue(currentData);
    }
}
