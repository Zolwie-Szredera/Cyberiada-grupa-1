using UnityEngine;

public class BloodBag : MonoBehaviour
{
    public int amount = 500;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PlayerHealth>().currentBlood < other.gameObject.GetComponent<PlayerHealth>().maxBlood)
        {
            other.gameObject.GetComponent<PlayerHealth>().GainBlood(amount);
            Destroy(gameObject);
        }
    }
}