using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugScript : MonoBehaviour
{
    public GameObject debugCanvas;
    public TextMeshProUGUI bloodText;
    private bool isDebugModeActive = false;
    private GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(player == null)
        {
            Debug.LogWarning("Player not found");
        }
        EnableOrDisableDebug(); //enabled on by default for now
    }
    void Update()
    {
        if (isDebugModeActive)
        {
            bloodText.text = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().currentBlood.ToString(); //bloodtext.text = current blood value
        }
    }
    public void OnEnableDebug(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        EnableOrDisableDebug();
    }
    public void EnableOrDisableDebug()
    {
        if (isDebugModeActive)
        {
            Debug.Log("DEBUG MODE DISABLED");
            debugCanvas.SetActive(false);
            isDebugModeActive = false;
        }
        else
        {
            Debug.Log("DEBUG MODE ENABLED. Click I for action 1, O for action 2, P for action 3");
            debugCanvas.SetActive(true);
            isDebugModeActive = true;
        }
    }
    //Change these during development to test stuff
    public void OnDebugAction1(InputAction.CallbackContext context) //Activate with: I
    {
        if (!isDebugModeActive) return;
        if (context.started)
        {
            Debug.Log("Debug action1");
            //put stuff here
            player.GetComponent<PlayerHealth>().GainBlackBile(10);
        }
    }
    public void OnDebugAction2(InputAction.CallbackContext context) //Activate with: O
    {
        if (!isDebugModeActive) return;
        if (context.started)
        {
            Debug.Log("Debug action2");
            //put stuff here

        }
    }
    public void OnDebugAction3(InputAction.CallbackContext context) //Activate with: P
    {
        if (!isDebugModeActive) return;
        if (context.started)
        {
            Debug.Log("Debug action3");
            //put stuff here

        }
    }
}

//Debug stuff is found in:
//here
//PlayerController.cs