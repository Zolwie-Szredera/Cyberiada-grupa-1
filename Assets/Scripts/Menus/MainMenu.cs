using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour

{
    public void OpenLevel(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void QuitGame()
    {
        Debug.Log("In a real game, this would quit the application.");
        Application.Quit();
    }
}
