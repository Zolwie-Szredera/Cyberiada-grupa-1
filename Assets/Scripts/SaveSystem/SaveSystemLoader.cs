using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// This script handles loading the saved game state after a scene is loaded
/// </summary>
public class SaveSystemLoader : MonoBehaviour
{
    private void Start()
    {
        LoadSavedGameState();
    }
    
    private void LoadSavedGameState()
    {
        SaveData saveData = SaveManager.Instance.LoadGame();
        if (saveData == null) return;
        
        // Check if we're in the correct scene
        if (saveData.sceneName != SceneManager.GetActiveScene().name)
        {
            return;
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        
        // Load player position
        player.transform.position = saveData.playerPosition;
        
        // Load player health
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.currentBlood = saveData.playerHealth;
        }
        
        // Load checkpoint
        CheckpointSystem checkpointSystem = player.GetComponent<CheckpointSystem>();
        if (checkpointSystem != null)
        {
            GameObject checkpoint = GameObject.Find(saveData.checkpointName);
            if (checkpoint != null)
            {
                checkpointSystem.currentCheckpoint = checkpoint;
            }
        }
        
        // Load accessories
        StartCoroutine(LoadAccessoriesDelayed(player, saveData));
    }
    
    private IEnumerator LoadAccessoriesDelayed(GameObject player, SaveData saveData)
    {
        // Wait for AccessoriesManager to initialize
        yield return null;
        
        AccessoriesManager accessoriesManager = player.GetComponent<AccessoriesManager>();
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        
        if (accessoriesManager == null || playerStats == null) yield break;
        
        // Load equipped accessories
        if (saveData.equippedAccessoryNames != null)
        {
            for (int i = 0; i < saveData.equippedAccessoryNames.Length && i < accessoriesManager.activeSlots.Length; i++)
            {
                string accessoryName = saveData.equippedAccessoryNames[i];
                if (!string.IsNullOrEmpty(accessoryName))
                {
                    Accessory accessory = Resources.Load<Accessory>("Accessories/" + accessoryName);
                    if (accessory != null)
                    {
                        accessoriesManager.activeSlots[i] = accessory;
                        playerStats.AddAccessory(accessory);
                    }
                }
            }
        }
        
        // Load inventory accessories
        if (saveData.inventoryAccessoryNames != null)
        {
            for (int i = 0; i < saveData.inventoryAccessoryNames.Length && i < accessoriesManager.inventory.Length; i++)
            {
                string accessoryName = saveData.inventoryAccessoryNames[i];
                if (!string.IsNullOrEmpty(accessoryName))
                {
                    Accessory accessory = Resources.Load<Accessory>("Accessories/" + accessoryName);
                    if (accessory != null)
                    {
                        accessoriesManager.inventory[i] = accessory;
                    }
                }
            }
        }
        
        Debug.Log("[SaveSystemLoader] Game state loaded successfully");
    }
}

