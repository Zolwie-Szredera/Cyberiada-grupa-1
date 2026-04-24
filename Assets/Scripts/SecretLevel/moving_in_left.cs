using UnityEngine;
using UnityEngine.UIElements;

public class moving_in_left : MonoBehaviour
{
    [Header("Ustawienia Ruchu")]
    public float speed = 5f; // Prêdkoœæ lotu w lewo

    void Update()
    {
        // Przesuwanie obiektu w lewo:
        // Vector3.left to skrót od (-1, 0, 0)
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // Opcjonalnie: Zniszcz obiekt, jeœli wyleci daleko poza ekran (np. X < -15)
        if (transform.position.x < -15f || transform.position.x > 15f)
        {
            Destroy(gameObject);
        }
    }
}