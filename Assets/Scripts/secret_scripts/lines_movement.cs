using UnityEngine;
using UnityEngine.InputSystem;

public class MuffetInputMovement : MonoBehaviour
{
    [Header("Linie (Przeci¹gnij obiekty z hierarchii)")]
    public Transform[] lineAnchors;
    private int currentLine = 1; // Start na œrodkowej linii

    [Header("Parametry Ruchu")]
    public float moveSpeed = 5f;
    public float transitionSpeed = 20f;
    public float boundaryX = 3f; // Szerokoœæ Twojego pude³ka

    private float horizontalInput = 0f;

    // --- LOGIKA SKOKU (Góra/Dó³) ---
    // Te metody podpinasz w Player Input -> Events

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
            if (lineAnchors != null && currentLine < lineAnchors.Length - 1)
                currentLine++;
        }
    }

    // --- LOGIKA RUCHU BOKIEM (Lewo/Prawo) ---

    public void OnGoLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Wciœniêto: LEWO");
            horizontalInput = -1f;
        }
        else if (context.canceled)
        {
            horizontalInput = 0f;
        }
    }

    public void OnGoRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Wciœniêto: PRAWO");
            horizontalInput = 1f;
        }
        else if (context.canceled)
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
        float targetY = transform.position.y; // Domyœlnie zostajemy na obecnym Y

        // Sprawdzamy, czy lista linii nie jest pusta, aby unikn¹æ b³êdu IndexOutOfRange
        if (lineAnchors != null && lineAnchors.Length > 0)
        {
            // Zabezpieczenie: upewnij siê, ¿e currentLine mieœci siê w tablicy
            currentLine = Mathf.Clamp(currentLine, 0, lineAnchors.Length - 1);

            targetY = lineAnchors[currentLine].position.y;
        }

        // 3. P³ynne przesuwanie serca
        float smoothY = Mathf.MoveTowards(transform.position.y, targetY, transitionSpeed * Time.deltaTime);

        // Aplikacja nowej pozycji
        transform.position = new Vector3(newX, smoothY, transform.position.z);
    }
}