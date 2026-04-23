using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic; // Wymagane dla Listy

public class lines_movement : MonoBehaviour
{
    [Header("Linie (Przeci¹gnij obiekty z hierarchii)")]
    public Transform[] lineAnchors;
    private int currentLine = 1;

    [Header("Parametry Ruchu")]
    public float moveSpeed = 5f;
    public float transitionSpeed = 20f;
    public float boundaryX = 3f;

    private float horizontalInput = 0f;

    // Lista przechowuj¹ca wciœniête kierunki w kolejnoœci ich naciskania
    private List<float> inputStack = new List<float>();

    // --- LOGIKA SKOKU (Góra/Dó³) ---

    public void OnGoUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentLine > 0) currentLine--;
        }
    }

    public void OnGoDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (lineAnchors != null && lineAnchors.Length > 0 && currentLine < lineAnchors.Length - 1)
                currentLine++;
        }
    }

    // --- LOGIKA RUCHU BOKIEM (Priorytet ostatniego klawisza) ---

    public void OnGoLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Dodaj -1 (lewo) na koniec listy
            if (!inputStack.Contains(-1f)) inputStack.Add(-1f);
        }
        else if (context.canceled)
        {
            // Usuñ -1 z listy gdy puœcisz klawisz
            inputStack.Remove(-1f);
        }
        UpdateHorizontalInput();
    }

    public void OnGoRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Dodaj 1 (prawo) na koniec listy
            if (!inputStack.Contains(1f)) inputStack.Add(1f);
        }
        else if (context.canceled)
        {
            // Usuñ 1 z listy gdy puœcisz klawisz
            inputStack.Remove(1f);
        }
        UpdateHorizontalInput();
    }

    private void UpdateHorizontalInput()
    {
        if (inputStack.Count > 0)
        {
            // horizontalInput to ostatni element dodany do listy
            horizontalInput = inputStack[inputStack.Count - 1];
        }
        else
        {
            horizontalInput = 0f;
        }
    }

    void Update()
    {
        float newX = transform.position.x + (horizontalInput * moveSpeed * Time.deltaTime);
        newX = Mathf.Clamp(newX, -boundaryX, boundaryX);

        float targetY = transform.position.y;
        if (lineAnchors != null && lineAnchors.Length > 0)
        {
            currentLine = Mathf.Clamp(currentLine, 0, lineAnchors.Length - 1);
            targetY = lineAnchors[currentLine].position.y;
        }

        float smoothY = Mathf.MoveTowards(transform.position.y, targetY, transitionSpeed * Time.deltaTime);
        transform.position = new Vector3(newX, smoothY, transform.position.z);
    }
}