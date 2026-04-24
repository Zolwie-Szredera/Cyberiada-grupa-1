using UnityEngine;

public class attack_upgrade : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        SecretPlayerShoot player = other.GetComponent<SecretPlayerShoot>();

        if (player != null)
        {
            // Dodajemy kule tylko jeœli ma mniej ni¿ 4
            if (player.upgradeLevel < 4)
            {
                player.upgradeLevel++;
                Debug.Log("Zebrano upgrade! Kule: " + player.upgradeLevel);
            }

            // Zniszcz przedmiot po zebraniu
            Destroy(gameObject);
        }
    }
}