using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(OptionsMenu))]
[RequireComponent(typeof(AudioSource))]
public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuCanvas;
    public GameObject optionsMenuCanvas;
    public GameObject mainCanvas;
    public GameObject tutorialCanvas;
    public bool isPaused = false;
    private GameObject player;
    private PlayerController playerController;
    private PlayerHealth playerHealth;
    private GameObject weapons;
    private bool playerDetected = true;
    private AudioSource audioSource;
    private PlayerInput playerInput;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //this script is also used in the main menu, so we need to check if there is a player in the scene before trying to access its components
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Debug.LogWarning("PauseMenu: No player found in the scene!");
            playerDetected = false;
            return;
        }
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        playerHealth = player.GetComponent<PlayerHealth>();
        playerInput = player.GetComponent<PlayerInput>();
        weapons = player.transform.Find("Weapons").gameObject;
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (GetComponent<OptionsMenu>().areYouSurePrompt.activeSelf)
        {
            Debug.Log("Cannot pause/unpause while the 'Are you sure?' prompt is active.");
            return;
        }
        if (!isPaused) //begin pause
        {
            if (playerDetected)
            {
                playerInput.actions.Disable();
                playerController.enabled = false;
                weapons.SetActive(false);
            }
            isPaused = true;
            Time.timeScale = 0;
            pauseMenuCanvas.SetActive(true);
            mainCanvas.SetActive(false);
            if (tutorialCanvas != null && tutorialCanvas.activeSelf)
            {
                tutorialCanvas.SetActive(false);
            }
        }
        else //end pause
        {
            if (optionsMenuCanvas.activeSelf)
            {
                GetComponent<OptionsMenu>().BackToMenu();
            }
            else
            {
                BackToGame();
            }
        }
        PlaySound();
    }
    //----------- BUTTONS -----------
    public void BackToGame()
    {
        if (playerDetected)
        {
            playerInput.actions.Enable();
            playerController.enabled = true;
            weapons.SetActive(true);
        }
        isPaused = false;
        Time.timeScale = 1;
        pauseMenuCanvas.SetActive(false);
        mainCanvas.SetActive(true);
        PlaySound();
    }
    public void OpenOptions()
    {
        pauseMenuCanvas.SetActive(false);
        optionsMenuCanvas.SetActive(true);
        PlaySound();
    }
    public void RestartToCheckpoint()
    {
        BackToGame();
        if(playerDetected)
        {
            playerHealth.Die();
            player.GetComponent<CheckpointSystem>().Respawn();
        }
        PlaySound();
    }
    public void QuitToMainMenu()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        //can't play sound here because the scene is changing, but we can play it in the main menu's start method
    }
    //----------- SOUND -----------
    private void PlaySound()
    {
        audioSource.Play();
    }
}
