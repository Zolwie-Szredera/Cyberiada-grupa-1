using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SecretPlayer : MonoBehaviour
{
    public float mainTimer = 0;
    public GameObject tutorialCanvas;
    // --- NOWE POLA DLA SERC ---
    [Header("UI Serca")]
    public GameObject[] hearts; // Przeciągnij tutaj swoje serca z hierarchii

    [Header("Linie (Przeciągnij obiekty z hierarchii)")]
    public Transform[] lineAnchors;
    private int currentLine = 1;

    [Header("Parametry Ruchu")]
    public float moveSpeed = 5f;
    public float transitionSpeed = 20f;
    public float boundaryX = 3f;

    [Header("Statystyki")]
    public float maxHealth = 5f; // Zmieniamy na 5, skoro mamy 5 serc
    private float currentHealth;
    public float immunityDuration = 1f;

    [Header("Efekt Odporności")]
    [SerializeField] private SpriteFlash spriteFlash;

    [Header("Death UI")]
    [SerializeField] private SecretLevel.SecretDeathUi deathUi;

    private float immuneTimer = 0;
    private float horizontalInput = 0f;
    private readonly List<float> inputStack = new();
    private bool _immuneVisualActive;
    private bool _isDead;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (spriteFlash == null)
        {
            spriteFlash = GetComponent<SpriteFlash>();
        }

        if (deathUi == null)
        {
            deathUi = FindAnyObjectByType<SecretLevel.SecretDeathUi>();
        }

        currentHealth = maxHealth;
        UpdateHeartsUI(); // Odśwież serca na starcie
    }

    // --- KLUCZOWA FUNKCJA DO AKTUALIZACJI UI ---
    void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            // Jeśli numer serca jest mniejszy niż obecne HP, pokaż je. W przeciwnym razie ukryj.
            if (i < currentHealth)
            {
                hearts[i].SetActive(true);
            }
            else
            {
                hearts[i].SetActive(false);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (_isDead)
        {
            return;
        }

        if (immuneTimer > 0)
        {
            SetImmuneVisual(true);
            return;
        }

        currentHealth -= amount;
        Debug.Log("HP: " + currentHealth);
        audioSource.Play();

        UpdateHeartsUI(); // AKTUALIZACJA PO OBRAŻENIACH

        immuneTimer = immunityDuration;
        SetImmuneVisual(true);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // --- RESZTA TWOJEGO KODU (OnGoUp, Update, etc.) ---
    // Pamiętaj, aby zachować pozostałe metody bez zmian, 
    // tylko upewnij się, że Update() korzysta z Twoich zmiennych.

    void Update()
    {
        if (_isDead)
        {
            return;
        }

        // Ruch poziomy i pionowy (twój istniejący kod)
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

        if (immuneTimer > 0) immuneTimer -= Time.deltaTime;
        if (immuneTimer <= 0f)
        {
            immuneTimer = 0f;
            SetImmuneVisual(false);
        }
        mainTimer += Time.deltaTime;
        if(mainTimer > 15f)
        {
            tutorialCanvas.SetActive(false);
        }
    }

    private void SetImmuneVisual(bool isEnabled)
    {
        if (_immuneVisualActive == isEnabled)
        {
            return;
        }

        _immuneVisualActive = isEnabled;
        if (spriteFlash != null)
        {
            spriteFlash.SetInverted(isEnabled);
        }
    }
    public void AddHealth(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        UpdateHeartsUI(); // To odświeży serca na ekranie
    }

    // ... (metody OnGoLeft, OnGoRight, UpdateHorizontalInput, Die pozostają bez zmian)

    public void OnGoUp(InputAction.CallbackContext context) { if (context.performed && currentLine > 0) currentLine--; }
    public void OnGoDown(InputAction.CallbackContext context) { if (context.performed && lineAnchors != null && currentLine < lineAnchors.Length - 1) currentLine++; }
    public void OnGoLeft(InputAction.CallbackContext context) { if (context.performed) { if (!inputStack.Contains(-1f)) inputStack.Add(-1f); } else if (context.canceled) inputStack.Remove(-1f); UpdateHorizontalInput(); }
    public void OnGoRight(InputAction.CallbackContext context) { if (context.performed) { if (!inputStack.Contains(1f)) inputStack.Add(1f); } else if (context.canceled) inputStack.Remove(1f); UpdateHorizontalInput(); }
    private void UpdateHorizontalInput() { if (inputStack.Count > 0) horizontalInput = inputStack[^1]; else horizontalInput = 0f; }
    void Die()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;
        horizontalInput = 0f;
        immuneTimer = 0f;
        SetImmuneVisual(false);

        if (deathUi == null)
        {
            deathUi = FindAnyObjectByType<SecretLevel.SecretDeathUi>();
        }

        if (deathUi != null)
        {
            deathUi.ShowDeathScreen();
        }

        Debug.Log("Gracz zginął!");
    }
}