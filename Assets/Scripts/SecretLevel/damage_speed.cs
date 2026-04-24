using System.Collections;
using UnityEngine;

public class damage_speed : MonoBehaviour
{
    [Header("Ustawienia Boostu")]
    [Tooltip("Wartoœæ, przez któr¹ mno¿ymy cooldown (np. 0.5 to dwa razy szybciej)")]
    public float cooldownMultiplier = 0.5f;
    public float duration = 4f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Szukamy skryptu strzelania na obiekcie, który wszed³ w kolizjê
        SecretPlayerShoot playerShoot = other.GetComponent<SecretPlayerShoot>();

        if (playerShoot != null)
        {
            // Startujemy Coroutine, która zajmie siê czasem trwania boosta
            StartCoroutine(ApplyBoost(playerShoot));

            // Sprawiamy, ¿e obiekt znika wizualnie i przestaje kolidowaæ
            DisableObject();
        }
    }

    private IEnumerator ApplyBoost(SecretPlayerShoot player)
    {
        // Zapamiêtujemy bazow¹ wartoœæ, ¿eby wiedzieæ do czego wróciæ
        float originalCooldown = player.shootCooldown;

        // Skracamy czas miêdzy strza³ami
        player.shootCooldown *= cooldownMultiplier;

        // Czekamy 4 sekundy
        yield return new WaitForSeconds(duration);

        // Przywracamy cooldown do normy
        player.shootCooldown = originalCooldown;

        // Dopiero teraz ca³kowicie usuwamy obiekt z pamiêci gry
        Destroy(gameObject);
    }

    private void DisableObject()
    {
        // Wy³¹czamy renderer i collider, ¿eby gracz myœla³, ¿e "zebra³" przedmiot
        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer sr)) sr.enabled = false;
        if (TryGetComponent<Collider2D>(out Collider2D col)) col.enabled = false;
    }
}