using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages transitions between game and menu based on save files
/// Should be on MainMenu Scene
/// </summary>
public class GameStateManager : MonoBehaviour
{
    private void Start()
    {
        // Ensure SaveManager exists
        if (FindAnyObjectByType<SaveManager>() == null)
        {
            GameObject saveMgr = new("SaveManager");
            saveMgr.AddComponent<SaveManager>();
        }
    }
    
    /// <summary>
    /// Load and navigate to the last saved scene
    /// </summary>
    public void ContinueGame()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("[GameStateManager] SaveManager not initialized!");
            return;
        }
        
        SaveData saveData = SaveManager.Instance.LoadGame();
        if (saveData != null && !string.IsNullOrEmpty(saveData.sceneName))
        {
            Debug.Log("[GameStateManager] Loading scene: " + saveData.sceneName);
            SceneManager.LoadScene(saveData.sceneName);
        }
        else
        {
            Debug.LogWarning("[GameStateManager] No valid save data found");
        }
    }
    
    /// <summary>
    /// Start a new game
    /// </summary>
    public void StartNewGame(string firstLevelSceneName = "Level1Courtyard")
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("[GameStateManager] SaveManager not initialized!");
            return;
        }
        
        // Delete previous save
        SaveManager.Instance.DeleteSaveFile();
        SaveManager.Instance.ResetPlayTime();
        
        Debug.Log("[GameStateManager] Starting new game. Loading scene: " + firstLevelSceneName);
        SceneManager.LoadScene(firstLevelSceneName);
    }
    
    /// <summary>
    /// Return to main menu
    /// </summary>
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Ensure time is resumed
        Debug.Log("[GameStateManager] Returning to main menu");
        SceneManager.LoadScene("MainMenu");
    }
    
    /// <summary>
    /// Check if a save file exists
    /// </summary>
    public bool HasSaveFile()
    {
        if (SaveManager.Instance != null)
        {
            return SaveManager.Instance.SaveFileExists();
        }
        return false;
    }
    
    /// <summary>
    /// Exit the game
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("[GameStateManager] Quitting game");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}



