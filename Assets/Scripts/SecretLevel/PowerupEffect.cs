using UnityEngine;

public class PowerupEffect : MonoBehaviour
{
    private GameObject player;
    public void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void DamageBoost()
    {
        player.GetComponent<SecretPlayerShoot>().damage += 1;
    }
    public void Invincibility()
    {
        player.GetComponent<SecretPlayer>().immuneTimer = 10;
    }
    public void Onslaught()
    {
        
    }
    public void Slowness()
    {
        player.GetComponent<SecretPlayer>().moveSpeed -= 1;
    }
    public void Saturation()
    {
        player.GetComponent<SecretPlayer>().currentHealth += 1;
    }
}