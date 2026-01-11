using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(OptionsMenu))]
public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuCanvas;
    public GameObject optionsMenuCanvas;
    public GameObject mainCanvas;
    public bool isPaused = false;
    private PlayerController playerController;
    private PlayerHealth playerHealth;
    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!isPaused) //begin pause
        {
            isPaused = true;
            playerController.enabled = false;
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
        isPaused = false;
        playerController.enabled = true;
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
        playerHealth.Die();
        // playerHealth.Respawn(); // <-- this doesnt exits on this branch. remove comment after merge with level-testing
    }
    public void QuitToMainMenu()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
