using UnityEngine;
using UnityEngine.InputSystem; // <-- NOWA LINIA: Wymagane do obs³ugi PlayerInput

[RequireComponent(typeof(OptionsMenu))]
public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuCanvas;
    public GameObject optionsMenuCanvas;
    public GameObject mainCanvas;
    public bool isPaused = false;
    private GameObject player;
    private PlayerController playerController;
    private PlayerHealth playerHealth;
    private GameObject weapons;
    private bool playerDetected = true;

    private PlayerInput playerInput;

    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Debug.LogWarning("PauseMenu: No player found in the scene!");
            playerDetected = false;
            return;
        }
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        playerHealth = player.GetComponent<PlayerHealth>();
        weapons = player.transform.Find("Weapons").gameObject;

        playerInput = player.GetComponent<PlayerInput>();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!isPaused)
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
        }
        else
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
    }


    public void OpenOptions()
    {
        pauseMenuCanvas.SetActive(false);
        optionsMenuCanvas.SetActive(true);
    }
    public void RestartToCheckpoint()
    {
        BackToGame();
        if (playerDetected)
        {
            playerHealth.Die();
            player.GetComponent<CheckpointSystem>().Respawn();
        }
    }
    public void QuitToMainMenu()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}