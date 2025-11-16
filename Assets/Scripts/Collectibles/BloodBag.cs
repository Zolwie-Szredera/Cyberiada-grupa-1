using UnityEngine;

public class BloodBag : MonoBehaviour
{
    public int amount = 500;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("blood bag triggered");
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().GainBlood(amount);
            Destroy(gameObject);
        }
    }
}