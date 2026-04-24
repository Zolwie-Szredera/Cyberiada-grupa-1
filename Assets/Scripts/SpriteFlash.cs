using UnityEngine;
using System.Collections;

public class SpriteFlash : MonoBehaviour
{
    [Header("Ustawienia Wizualne")]
    [Tooltip("Materiał inwertujący kolory lub zwykły biały materiał")]
    [SerializeField] private Material flashMaterial; 
    
    [Tooltip("Czas trwania błysku w sekundach")]
    [SerializeField] private float duration = 0.1f;

    private SpriteRenderer spriteRenderer;  
    private Material originalMaterial;
    private Coroutine flashCoroutine;
    private bool isInverted;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    /// <summary>
    /// Metoda wywoływana z innych skryptów (np. z Dash.cs)
    /// </summary>
    public void CallFlash()
    {
        // Jeśli poprzedni błysk jeszcze trwa, przerwij go
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        // Uruchom błysk
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    public void SetInverted(bool isEnabled)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (isEnabled)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
                flashCoroutine = null;
            }

            spriteRenderer.material = flashMaterial;
            isInverted = true;
            return;
        }

        spriteRenderer.material = originalMaterial;
        isInverted = false;
    }

    private IEnumerator FlashRoutine()
    {
        // 1. Zmień materiał na flash
        spriteRenderer.material = flashMaterial;

        // 2. Odczekaj określony czas
        yield return new WaitForSeconds(duration);

        // 3. Przywróć oryginalny materiał
        if (!isInverted)
        {
            spriteRenderer.material = originalMaterial;
        }

        // Wyczyść referencję do coroutine
        flashCoroutine = null;
    }
}