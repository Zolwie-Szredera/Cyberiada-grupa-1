using UnityEngine;

public class line_health : MonoBehaviour // Zmieñ na Line_health, jeœli tak nazywa siê plik
{
    [Header("Statystyki")]
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        // Na starcie ustawiamy HP na maksimum
        currentHealth = maxHealth;
    }

    // Metoda do zadawania obra¿eñ, któr¹ mo¿esz wywo³aæ z innych skryptów
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("Gracz otrzyma³ obra¿enia. Aktualne HP: " + currentHealth);

        // Sprawdzenie czy gracz powinien znikn¹æ
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Gracz zgin¹³!");

        // Niszczy obiekt gracza
        Destroy(gameObject);

        // Opcjonalnie: Tutaj mo¿esz dodaæ kod na spawn efektu wybuchu 
        // lub pokazanie ekranu "Game Over"
    }
}