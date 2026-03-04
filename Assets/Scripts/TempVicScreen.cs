using UnityEngine;

public class TempVicScreen : Button
{
    private Canvas[] allCanvases;
    public Canvas victoryScreen;
    public override void Interaction()
    {
        allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach(Canvas canvas in allCanvases)
        {
            canvas.gameObject.SetActive(false);
        }
        victoryScreen.gameObject.SetActive(true);
    }
}
