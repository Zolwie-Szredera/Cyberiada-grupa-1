using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Save system integration with pause menu
/// Allows returning to menu and resuming game
/// </summary>
public class PauseMenuSaveIntegration : MonoBehaviour
{
    /// <summary>
    /// Save game and return to menu
    /// </summary>
    public void SaveAndReturnToMenu()
    {
        if (SaveManager.Instance != null)
        {
            // Save current state
            SaveManager.Instance.SaveGame();
            Debug.Log("[PauseMenuSaveIntegration] Game saved before returning to menu");
        }
        
        // Reset time
        Time.timeScale = 1f;
        
        // Return to menu
        SceneManager.LoadScene("MainMenu");
    }
    
    /// <summary>
    /// Resume game from last save
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Debug.Log("[PauseMenuSaveIntegration] Game resumed");
    }
    
    /// <summary>
    /// Return to menu without saving
    /// </summary>
    public void ReturnToMenuWithoutSave()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

