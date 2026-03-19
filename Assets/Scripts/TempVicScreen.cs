using UnityEngine;
using UnityEngine.SceneManagement;

public class TempVicScreen : Button
{
    private Canvas[] allCanvases;
    public Canvas victoryScreen;
    private bool victoryActive = false;
    private float timer = 0f;
    private bool autoTransitionStarted = false;

    public override void Interaction()
    {
        allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach(Canvas canvas in allCanvases)
        {
            canvas.gameObject.SetActive(false);
        }
        victoryScreen.gameObject.SetActive(true);
        victoryActive = true;
        timer = 0f;
        autoTransitionStarted = false;
    }

    void Update()
    {
        if (!victoryActive) return;

        // Auto-transition to main menu after 5 seconds
        timer += Time.unscaledDeltaTime;
        if (timer >= 5f && !autoTransitionStarted)
        {
            autoTransitionStarted = true;
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu");
            victoryActive = false;
        }
    }
}
