using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Button : MonoBehaviour
{
    [HideInInspector]public GameObject interactText;
    private PlayerController playerController;
    private bool readyToInteract = false;
    void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        readyToInteract = true;
        playerController.ActivateInteractionText(true);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        readyToInteract = false;
        playerController.ActivateInteractionText(false);
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
    }
}