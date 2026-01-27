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
        
        // Try to auto-discover UI elements if not assigned
        if (bloodFill == null)
        {
            bloodFill = FindUIElement("BloodFill") ?? FindUIElement("BloodSlider/Fill Area/Fill");
            if (bloodFill == null)
            {
                Debug.LogWarning("PlayerHealth: bloodFill Image is not assigned and could not be found automatically! Health UI will not display.");
            }
            else
            {
                Debug.Log("PlayerHealth: Auto-discovered bloodFill UI element.");
            }
        }
        
        if (blackBileFill == null)
        {
            blackBileFill = FindUIElement("BlackBileFill") ?? FindUIElement("BlackBileSlider/Fill Area/Fill");
            if (blackBileFill == null)
            {
                Debug.LogWarning("PlayerHealth: blackBileFill Image is not assigned and could not be found automatically! Black Bile UI will not display.");
            }
            else
            {
                Debug.Log("PlayerHealth: Auto-discovered blackBileFill UI element.");
                blackBileFill.fillAmount = 0;
            }
        }
        else
        {
            blackBileFill.fillAmount = 0;
        }
    }
    
    private Image FindUIElement(string path)
    {
        GameObject obj = GameObject.Find(path);
        if (obj != null)
        {
            return obj.GetComponent<Image>();
        }
        return null;
    }
    void Update()
    {
        if (bloodFill != null)
        {
            bloodFill.fillAmount = currentBlood / maxBlood;
        }
        if(currentBlackBile > 0)
        {
            if (blackBileFill != null)
            {
                blackBileFill.fillAmount = currentBlackBile / maxBlood;
            }
            currentBlackBile -= Time.deltaTime;
        }
    }
    public void Die()
    {
        playerController.enabled = false;
        Time.timeScale = 0;
        Debug.Log("You died!");
    }
    public void TakeDamage(float damage)
    {
        currentBlood -= damage;
        Debug.Log($"Player took {damage} damage. Current health: {currentBlood}/{maxBlood}");
        
        if (bloodFill != null)
        {
            bloodFill.fillAmount = currentBlood / maxBlood;
        }
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
