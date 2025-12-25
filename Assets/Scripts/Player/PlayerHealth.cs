using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealth : MonoBehaviour
{
    public float maxBlood = 100;
    private PlayerController playerController;
    public Image bloodFill;
    public Image blackBileFill;
    [HideInInspector] public float currentBlackBile;
    [HideInInspector] public float currentBlood;
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        currentBlood = maxBlood;
        currentBlackBile = 0;
        blackBileFill.fillAmount = 0;
    }
    void Update()
    {
        bloodFill.fillAmount = currentBlood / maxBlood;
        if(currentBlackBile > 0)
        {
            blackBileFill.fillAmount = currentBlackBile / maxBlood;
            currentBlackBile -= Time.deltaTime;
        }
    }
    public void Die()
    {
        playerController.enabled = false;
        Time.timeScale = 0;
        Debug.Log("You died!");
    }
    public void TakeDamage(int damage)
    {
        currentBlood -= damage;
        bloodFill.fillAmount = currentBlood / maxBlood;
        if (currentBlood <= 0)
        {
            Die();
        }
    }
    // BLOOD (WIP)
    // Get it from enemies
    public void GainBlood(float gain) //use instead of blood += n as it is likely that gaining blood will cause additional effects in the future.
    {
        currentBlood += gain;
        if (currentBlood > maxBlood || currentBlackBile != 0)
        {
            currentBlood = Mathf.Clamp(currentBlood, 0, maxBlood - currentBlackBile);
        }
        Debug.Log("Gained blood: " + gain);
    }
    //BLACK BILE status effect. Is this interesting enough?
    public void GainBlackBile(float amount)
    {
        currentBlackBile += amount;
        currentBlackBile = Mathf.Clamp(currentBlackBile, 0f, maxBlood - currentBlood);
    }
}
