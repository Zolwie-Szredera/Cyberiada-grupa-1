using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SaveSystemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI continueButtonText;
    [SerializeField] private UnityEngine.UI.Button continueButton;
    [SerializeField] private TextMeshProUGUI saveInfoText;
    
    private void Start()
    {
        UpdateSaveUI();
        
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }
    }
    
    private void UpdateSaveUI()
    {
        if (SaveManager.Instance == null) return;
        
        bool saveExists = SaveManager.Instance.SaveFileExists();
        
        if (continueButton != null)
        {
            continueButton.interactable = saveExists;
        }
        
        if (saveInfoText != null)
        {
            if (saveExists)
            {
                SaveData saveData = SaveManager.Instance.LoadGame();
                if (saveData != null)
                {
                    saveInfoText.text = "Last save: " + saveData.sceneName + " (" + saveData.saveTime + ")";
                }
                }
            else
            {
                saveInfoText.text = "No save found";
            }
        }
    }
    
    public void OnNewGameButtonClicked()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.DeleteSaveFile();
            SaveManager.Instance.ResetPlayTime();
        }
        
        SceneManager.LoadScene("Level1Courtyard");
    }
    
    public void OnContinueButtonClicked()
    {
        if (SaveManager.Instance == null) return;
        
        SaveData saveData = SaveManager.Instance.LoadGame();
        if (saveData != null && !string.IsNullOrEmpty(saveData.sceneName))
        {
            SceneManager.LoadScene(saveData.sceneName);
        }
        else
        {
            Debug.LogError("[SaveSystemUI] Failed to load game data");
        }
    }
}



