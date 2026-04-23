using UnityEngine;
using UnityEngine.InputSystem;

public class MuffetInputMovement : MonoBehaviour
{
    [Header("Linie (Przeci¹gnij obiekty z hierarchii)")]
    public Transform[] lineAnchors;
    private int currentLine = 1;

    [Header("Parametry Ruchu")]
    public float moveSpeed = 5f;
    public float transitionSpeed = 20f;
    public float boundaryX = 3f;

    private float horizontalInput = 0f;

    // Zmienne do œledzenia stanu klawiszy
    private bool isLeftPressed = false;
    private bool isRightPressed = false;

    // --- LOGIKA SKOKU (Góra/Dó³) ---

    public void OnGoUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Klikniêto: GÓRA");
            if (currentLine > 0)
                currentLine--;
        }
    }

    public void OnGoDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Klikniêto: DÓ£");
            // Dodano bezpieczne sprawdzenie d³ugoœci tablicy
            if (lineAnchors != null && lineAnchors.Length > 0 && currentLine < lineAnchors.Length - 1)
                currentLine++;
        }
    }

    // --- LOGIKA RUCHU BOKIEM (Lewo/Prawo) ---

    public void OnGoLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Wciœniêto: LEWO");
            isLeftPressed = true;
        }
        else if (context.canceled)
        {
            isLeftPressed = false;
        }

        UpdateHorizontalInput();
    }

    public void OnGoRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Wciœniêto: PRAWO");
            isRightPressed = true;
        }
        else if (context.canceled)
        {
            isRightPressed = false;
        }

        UpdateHorizontalInput();
    }

    // Decyduje o kierunku na podstawie wciœniêtych klawiszy
    private void UpdateHorizontalInput()
    {
        if (isLeftPressed && isRightPressed)
        {
            // Jeœli trzymasz oba, stoisz w miejscu (jak w Undertale)
            horizontalInput = 0f;
        }
        else if (isLeftPressed)
        {
            horizontalInput = -1f;
        }
        else if (isRightPressed)
        {
            horizontalInput = 1f;
        }
        else
        {
            horizontalInput = 0f;
        }
    }

    void Update()
    {
        // 1. Obliczanie pozycji poziomej (X)
        float newX = transform.position.x + (horizontalInput * moveSpeed * Time.deltaTime);
        newX = Mathf.Clamp(newX, -boundaryX, boundaryX);

        // 2. Obliczanie pozycji pionowej (Y)
        float targetY = transform.position.y;

        if (lineAnchors != null && lineAnchors.Length > 0)
        {
            currentLine = Mathf.Clamp(currentLine, 0, lineAnchors.Length - 1);
            targetY = lineAnchors[currentLine].position.y;
        }

        // 3. P³ynne przesuwanie serca
        float smoothY = Mathf.MoveTowards(transform.position.y, targetY, transitionSpeed * Time.deltaTime);

        // Aplikacja nowej pozycji (z zachowaniem oryginalnego Z)
        transform.position = new Vector3(newX, smoothY, transform.position.z);
    }
}