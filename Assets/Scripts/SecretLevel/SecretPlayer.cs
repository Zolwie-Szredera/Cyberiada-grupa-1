using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic; // Wymagane dla Listy

public class SecretPlayer : MonoBehaviour
{
    [Header("Linie (Przecignij obiekty z hierarchii)")]
    public Transform[] lineAnchors;
    private int currentLine = 1;

    [Header("Parametry Ruchu")]
    public float moveSpeed = 5f;
    public float transitionSpeed = 20f;
    public float boundaryX = 3f;
    [Header("Statystyki")]
    public float maxHealth = 100f;
    private float currentHealth;
    private float horizontalInput = 0f;
    public float immunityDuration;

    private float immuneTimer = 0;
    private readonly List<float> inputStack = new();

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
            // Usu� -1 z listy gdy pu�cisz klawisz
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
            // Usu� 1 z listy gdy pu�cisz klawisz
            inputStack.Remove(1f);
        }
        UpdateHorizontalInput();
    }

    private void UpdateHorizontalInput()
    {
        if (inputStack.Count > 0)
        {
            // horizontalInput to ostatni element dodany do listy
            horizontalInput = inputStack[^1];
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

        if(immuneTimer > 0)
        {
            immuneTimer -= Time.deltaTime;
        }
    }

    void Start()
    {
        // Na starcie ustawiamy HP na maksimum
        currentHealth = maxHealth;
    }

    // Metoda do zadawania obra�e�, kt�r� mo�esz wywo�a� z innych skrypt�w
    public void TakeDamage(float amount)
    {
        if(immuneTimer > 0)
        {
            return;
        }
        currentHealth -= amount;
        Debug.Log("Gracz otrzymal obrazenia: " + amount +  ". Aktualne HP: " + currentHealth);
        immuneTimer = immunityDuration;
        // Sprawdzenie czy gracz powinien znikn��
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Gracz zgin��!");

        // Niszczy obiekt gracza
        Destroy(gameObject);

        // Opcjonalnie: Tutaj mo�esz doda� kod na spawn efektu wybuchu 
        // lub pokazanie ekranu "Game Over"
    }
}