using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugScript : MonoBehaviour
{
    public GameObject debugCanvas;
    public TextMeshProUGUI bloodText;
    public TextMeshProUGUI airJumpText;
    public TextMeshProUGUI mousePositionTextX;
    public TextMeshProUGUI mousePositionTextY;
    public TextMeshProUGUI playerVelocityTextX;
    public TextMeshProUGUI playerVelocityTextY;
    public AudioSource audioSource;
    private bool isDebugModeActive = false;

    private GameObject player;
    private GameObject gameManager;
    void Start() //no need to check for null here since the script won't work without them and will throw an error anyway
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        EnableOrDisableDebug(); //enabled on by default for now
    }
    void Update()
    {
        if (isDebugModeActive)
        {
            airJumpText.text = player.GetComponent<PlayerController>().remainingAirJumps.ToString();
            if (player.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                bloodText.text = playerHealth.currentBlood.ToString(); //bloodtext.text = current blood value
            }
            if (gameManager.TryGetComponent<GameManager>(out var gm))
            {
                mousePositionTextX.text = "X: " + gm.mousePosition.x;
                mousePositionTextY.text = "Y: " + gm.mousePosition.y;
            }
            if (player.TryGetComponent<Rigidbody2D>(out var rb))
            {
                playerVelocityTextX.text = rb.linearVelocityX.ToString("F3");
                playerVelocityTextY.text = rb.linearVelocityY.ToString("F3");
            }
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
            audioSource.Play();
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