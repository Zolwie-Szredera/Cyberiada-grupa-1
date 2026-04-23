using UnityEngine;

public class take_damage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f;



    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Kolizja z: " + other.gameObject.name);
        // ZMIANA: Tutaj tak samo
        line_health player = other.gameObject.GetComponent<line_health>();

        if (player != null)
        {
            player.TakeDamage(damageAmount);
        }
    }
}