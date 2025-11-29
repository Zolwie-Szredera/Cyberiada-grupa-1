using TMPro;
using Unity.Multiplayer.Center.Common.Analytics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Button : MonoBehaviour
{
    [HideInInspector]public GameObject interactText;
    private bool readyToInteract = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        interactText = GameObject.FindGameObjectWithTag("InteractText");
        interactText.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        interactText.SetActive(true);
        readyToInteract = true;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        interactText.SetActive(false);
        readyToInteract = false;
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if(readyToInteract && context.started)
        {
            Interaction();
        }
    }
    public virtual void Interaction()
    {
        Debug.Log("Interaction!");
    }
}
