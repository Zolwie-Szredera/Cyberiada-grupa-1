using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealth : MonoBehaviour
{
    public float maxBlood = 1000;
    private PlayerController playerController;
    public float currentBlood;
    public Slider bloodSlider;
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        currentBlood = maxBlood;
        bloodSlider.maxValue = maxBlood;
        bloodSlider.value = currentBlood;
    }
    void Update()
    {
        bloodSlider.value = currentBlood;
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
        bloodSlider.value = currentBlood;
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
        if (currentBlood > maxBlood)
        {
            currentBlood = Mathf.Clamp(currentBlood, 0, maxBlood);
        }
        Debug.Log("Gained blood: " + gain);
    }
}
