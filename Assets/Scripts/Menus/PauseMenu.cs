using UnityEngine;
using UnityEngine.InputSystem;

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
    private void Start()
    {
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
        weapons = player.transform.Find("Weapons").gameObject;
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!isPaused) //begin pause
        {
            if (playerDetected)
            {
                playerController.enabled = false;
                weapons.SetActive(false);
            }
            isPaused = true;
            Time.timeScale = 0;
            pauseMenuCanvas.SetActive(true);
            mainCanvas.SetActive(false);
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
    }
    //----------- BUTTONS -----------
    public void BackToGame()
    {
        if (playerDetected)
        {
            playerController.enabled = true;
            weapons.SetActive(true);
        }
        isPaused = false;
        Time.timeScale = 1;
        pauseMenuCanvas.SetActive(false);
    }
    public void OpenOptions()
    {
        pauseMenuCanvas.SetActive(false);
        optionsMenuCanvas.SetActive(true);
    }
    public void RestartToCheckpoint()
    {
        BackToGame();
        if(playerDetected)
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
