using UnityEngine;

public class Saturation : MonoBehaviour
{
    public float healAmount = 1f;

    // U¿ywamy OnTriggerEnter2D, bo Twój Enemy te¿ go u¿ywa (wiêc masz grê 2D)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<SecretPlayer>(out var player))
        {
            player.AddHealth(healAmount); // Leczymy gracza
            Destroy(gameObject);        // Usuwamy "jedzenie" z mapy
        }
    }
}