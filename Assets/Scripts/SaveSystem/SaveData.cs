using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string sceneName;
    public string checkpointName;
    public Vector3 playerPosition;
    public float playerHealth;
    
    // Accessories data - Arrays for JSON serialization
    public string[] equippedAccessoryNames;
    public string[] inventoryAccessoryNames;

    // Ability unlocks
    public bool isDashUnlocked;
    public bool isDoubleJumpUnlocked;
    
    // Timestamp
    public string saveTime;
    public int playTime; // in seconds
}


