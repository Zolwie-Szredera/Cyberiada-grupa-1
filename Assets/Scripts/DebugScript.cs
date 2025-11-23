using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugScript : MonoBehaviour
{
    public GameObject debugCanvas;
    public TextMeshProUGUI bloodText;
    private bool isDebugModeActive = false;
    void Start()
    {
        EnableOrDisableDebug(); //enabled on by default for now
    }
    void Update()
    {
        if(isDebugModeActive)
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
            Debug.Log("DEBUG MODE ENABLED");
            debugCanvas.SetActive(true);
            isDebugModeActive = true;
        }
    }
}

//Debug stuff is found in:
//here
//PlayerController.cs