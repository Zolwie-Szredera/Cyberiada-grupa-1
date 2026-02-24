using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour

{
    public Button[] buttons;
    //private void Awake()
    //
    //{
    //    int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
    //    for (int i = 0; i < buttons.Length; i++)
    //    {
    //        buttons[i].interactable = false;
    //    }
    //    for (int i = 0; i < unlockedLevel; i++)
    //    {
    //        buttons[i].interactable = true;
    //    }
    //}
    public void OpenLevel(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
