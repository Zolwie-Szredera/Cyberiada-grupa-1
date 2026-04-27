using UnityEngine;
using UnityEngine.UI;

public class SaveSystemDebug : MonoBehaviour
{
    [SerializeField] private Text debugText;
    
    private void Update()
    {
        if (debugText != null)
        {
            UpdateDebugDisplay();
        }
    }
    
    private void UpdateDebugDisplay()
    {
        string text = "[SAVE SYSTEM DEBUG]\n\n";
        
        if (SaveManager.Instance == null)
        {
            text += "❌ SaveManager.Instance: NULL\n";
        }
        else
        {
            text += "✓ SaveManager.Instance: Initialized\n";
            text += "✓ Save File Exists: " + SaveManager.Instance.SaveFileExists() + "\n";
            
            if (SaveManager.Instance.SaveFileExists())
            {
                SaveData data = SaveManager.Instance.GetSaveFileInfo();
                if (data != null)
                {
                    text += "\n--- Last Save Info ---\n";
                    text += "Scene: " + data.sceneName + "\n";
                    text += "Checkpoint: " + data.checkpointName + "\n";
                    text += "Time: " + data.saveTime + "\n";
                    text += "Play Time: " + data.playTime + "s\n";
                    text += "Equipped Accessories: " + (data.equippedAccessoryNames != null ? data.equippedAccessoryNames.Length : 0) + "\n";
                    text += "Inventory Items: " + (data.inventoryAccessoryNames != null ? data.inventoryAccessoryNames.Length : 0) + "\n";
                }
            }
            else
            {
                text += "ℹ No save file found\n";
            }
        }
        
        // Check for SaveSystemLoader
        SaveSystemLoader loader = FindAnyObjectByType<SaveSystemLoader>();
        if (loader != null)
        {
            text += "\n✓ SaveSystemLoader: Found on scene\n";
        }
        else
        {
            text += "\n⚠ SaveSystemLoader: NOT found on scene\n";
        }
        
        debugText.text = text;
    }
    
    // UI Buttons for debugging
    public void DebugSaveGame()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
            Debug.Log("[SaveSystemDebug] Manual save executed");
        }
    }
    
    public void DebugLoadGame()
    {
        if (SaveManager.Instance != null)
        {
            SaveData data = SaveManager.Instance.LoadGame();
            if (data != null)
            {
                Debug.Log("[SaveSystemDebug] Save loaded successfully");
                Debug.Log("Scene: " + data.sceneName);
                Debug.Log("Checkpoint: " + data.checkpointName);
            }
            else
            {
                Debug.Log("[SaveSystemDebug] No save file to load");
            }
        }
    }
    
    public void DebugDeleteSave()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.DeleteSaveFile();
            Debug.Log("[SaveSystemDebug] Save file deleted");
        }
    }
    
    public void DebugShowPath()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "gamesave.json");
        Debug.Log("[SaveSystemDebug] Save path: " + path);
    }
}




