using System.Collections;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    //1. Move camera to focus point
    //2. Start dialogue if needed
    //3. When dialogue ends, move camera back to player
    public DialogueData dialogueData;
    public Transform cameraFocusPoint;
    private Camera mainCamera;
    private DialogueHandler dialogueHandler;
    private Vector3 originalCameraPosition;
    void Start()
    {
        mainCamera = Camera.main;
        dialogueHandler = GameObject.FindGameObjectWithTag("GameManager").GetComponentInChildren<DialogueHandler>();
    }
    //override this in child classes
    public virtual void StartCutscene()
    {
        Debug.Log("You forgot to override StartCutscene in " + gameObject.name);
    }
    public virtual void EndCutscene()
    {
        Debug.Log("You forgot to override EndCutscene in " + gameObject.name);
    }
    public virtual void OnDialogueEnd()
    {
        Debug.Log("You forgot to override OnDialogueEnd in " + gameObject.name);
        //This is called by DialogueHandler when dialogue ends, so you can override it in child classes to do something after dialogue ends
    }
    protected IEnumerator MoveCameraToFocusPoint()
    {
        originalCameraPosition = mainCamera.transform.position;
        float duration = 2f; // Duration of the cutscene
        float elapsedTime = 0f;
        Vector3 startingPosition = mainCamera.transform.position;

        while (elapsedTime < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(startingPosition, cameraFocusPoint.position, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        mainCamera.transform.position = cameraFocusPoint.position; // Ensure it ends exactly at the focus point
    }
    protected IEnumerator MoveCameraBackToPlayer()
    {
        float duration = 2f; // Duration of the cutscene
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            //interpolate from cameraFocusPoint.position to originalCameraPosition
            mainCamera.transform.position = Vector3.Lerp(cameraFocusPoint.position, originalCameraPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        mainCamera.transform.position = originalCameraPosition; // Ensure it ends exactly at the original camera position
    }
    protected void StartDialogue()
    {
        dialogueHandler.StartDialogue(dialogueData, null, this);
    }
}
