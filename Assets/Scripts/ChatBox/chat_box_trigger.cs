using UnityEngine;
using UnityEngine.InputSystem;

public class chat_box_trigger : MonoBehaviour
{
    public chat_box chatBox;
    public DialogueData dialogueToLoad;
    private bool playerInRange;

    void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) playerInRange = true; }
    void OnTriggerExit2D(Collider2D other) { if (other.CompareTag("Player")) playerInRange = false; }

    void Update()
    {
        if (playerInRange && Keyboard.current.zKey.wasPressedThisFrame)
        {
            // Odpalamy tylko gdy chat nie jest ju¿ otwarty
            if (chatBox != null && !chatBox.gameObject.activeInHierarchy)
            {
                chatBox.StartDialogue(dialogueToLoad);
            }
        }
    }
}