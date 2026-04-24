using UnityEngine;

public class moving_in_right : MonoBehaviour
{
    [Header("Ustawienia Ruchu")]
    public float speed = 5f; // Prêdkoœæ lotu w lewo

    void Update()
    {
        // Przesuwanie obiektu w lewo:
        // Vector3.left to skrót od (-1, 0, 0)
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Opcjonalnie: Zniszcz obiekt, jeœli wyleci daleko poza ekran (np. X < -15)
        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
}