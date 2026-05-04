using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    
    private string savePath;
    private const string SaveFileName = "gamesave.json";
    private float playTimeCounter = 0f;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        savePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        Debug.Log($"[SaveManager] Save path: {savePath}");
    }
    
    private void Update()
    {
        playTimeCounter += Time.deltaTime;
    }
    
    /// <summary>
    /// Save the current game state
    /// </summary>
    public void SaveGame()
    {
        SaveData saveData = new SaveData();
        
        // Scene data
        saveData.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        // Checkpoint data
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CheckpointSystem checkpointSystem = player.GetComponent<CheckpointSystem>();
            if (checkpointSystem != null && checkpointSystem.currentCheckpoint != null)
            {
                saveData.checkpointName = checkpointSystem.currentCheckpoint.name;
                saveData.playerPosition = player.transform.position;
            }
            
            // Player health
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                saveData.playerHealth = playerHealth.currentBlood;
            }
            
            // Accessories data
            AccessoriesManager accessoriesManager = player.GetComponent<AccessoriesManager>();
            if (accessoriesManager != null)
            {
                // Save equipped accessories
                saveData.equippedAccessoryNames = new string[accessoriesManager.activeSlots.Length];
                for (int i = 0; i < accessoriesManager.activeSlots.Length; i++)
                {
                    saveData.equippedAccessoryNames[i] = 
                        (accessoriesManager.activeSlots[i] != null) 
                        ? accessoriesManager.activeSlots[i].name 
                        : "";
                }
                
                // Save inventory accessories
                saveData.inventoryAccessoryNames = new string[accessoriesManager.inventory.Length];
                for (int i = 0; i < accessoriesManager.inventory.Length; i++)
                {
                    saveData.inventoryAccessoryNames[i] = 
                        (accessoriesManager.inventory[i] != null) 
                        ? accessoriesManager.inventory[i].name 
                        : "";
                }
            }

            saveData.isDashUnlocked = PlayerStats.isDashUnlocked;
            saveData.isDoubleJumpUnlocked = PlayerStats.isDoubleJumpUnlocked;
        }
        
        // Timestamp
        saveData.saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        saveData.playTime = (int)playTimeCounter;
        
        // Serialize to JSON
        string json = JsonUtility.ToJson(saveData, true);
        
        try
        {
            File.WriteAllText(savePath, json);
            Debug.Log($"[SaveManager] Game saved successfully at {savePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveManager] Failed to save game: {e.Message}");
        }
    }
    
    /// <summary>
    /// Load the saved game state
    /// </summary>
    public SaveData LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning($"[SaveManager] Save file not found at {savePath}");
            return null;
        }
        
        try
        {
            string json = File.ReadAllText(savePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"[SaveManager] Game loaded successfully");
            return saveData;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveManager] Failed to load game: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Check if a save file exists
    /// </summary>
    public bool SaveFileExists()
    {
        return File.Exists(savePath);
    }
    
    /// <summary>
    /// Delete the save file
    /// </summary>
    public void DeleteSaveFile()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log($"[SaveManager] Save file deleted");
        }
    }
    
    /// <summary>
    /// Get the save file info (for UI display)
    /// </summary>
    public SaveData GetSaveFileInfo()
    {
        return LoadGame();
    }
    
    /// <summary>
    /// Reset play time counter (for new game)
    /// </summary>
    public void ResetPlayTime()
    {
        playTimeCounter = 0f;
    }
}

