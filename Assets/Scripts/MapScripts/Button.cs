using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Button : MonoBehaviour
{
    public bool executeActionOnInteract = false;
    public Action[] actions;
    [HideInInspector]public GameObject interactText;
    private PlayerController playerController;
    private bool readyToInteract = false;
    void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            readyToInteract = true;
            playerController.ActivateInteractionText(true);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            readyToInteract = false;
            playerController.ActivateInteractionText(false);
        }
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if(readyToInteract && context.started)
        {
            Interaction();
        }
    }
    //remember to re-add player input -> events -> player when you change this in any way or use a script that inherits from this.
    public virtual void Interaction()
    {
        Debug.Log("Interaction!");
        if(executeActionOnInteract)
        {
            ExecuteAllActions();
        }
        //put more stuff after base.Interaction() in scripts that inherit from button.cs cuz base does *nothing
    }
    //store Actions
    public void ExecuteAllActions()
    {
        if(actions.Length == 0)
        {
            Debug.LogWarning("no actions to execute");
        }
        foreach(Action action in actions)
        {
            action.ExecuteAction();
        }
    }
}