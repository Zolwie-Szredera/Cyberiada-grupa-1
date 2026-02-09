using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//I made this script specifically for the "Reset to default" button in Options Menu
public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float holdTime = 3f;
    public Image fillImage;
    public OptionsMenu optionsMenu;
    private Color normalColor = Color.white;
    private Color fillColor = Color.red;
    private bool isHolding = false;
    private float holdTimer = 0f;

    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(holdTimer / holdTime);
            fillImage.fillAmount = progress;
            fillImage.color = Color.Lerp(normalColor, fillColor, progress);
            if (holdTimer >= holdTime)
            {
                isHolding = false;
                holdTimer = 0f;
                fillImage.fillAmount = 0f;
                fillImage.color = normalColor;
                optionsMenu.ResetAllSettings();
                Debug.Log("Settings reset");
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData) //when pointer (cursor)is down
    {
        isHolding = true;
        holdTimer = 0f;
        fillImage.fillAmount = 0f;
        fillImage.color = normalColor;
    }

    public void OnPointerUp(PointerEventData eventData) //if pointer is released before time
    {
        isHolding = false;
        holdTimer = 0f;
        fillImage.fillAmount = 0f;
        fillImage.color = normalColor;
    }
}