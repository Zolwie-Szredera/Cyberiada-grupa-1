using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 20;
    public int currentHealth;
    public float maxBlood = 1000;
    private PlayerController playerController;
    public float currentBlood;
    public Slider bloodSlider;
    public float bloodLossRate = 15;
    private bool bloodOverflow;
    private float bloodOverflowTimeIncrease;
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        currentBlood = 0;
        currentHealth = maxHealth;
    }
    void Update()
    {
        if (!bloodOverflow)
        {
            currentBlood -= Time.deltaTime * bloodLossRate;
        }
        currentBlood = Mathf.Clamp(currentBlood, 0, maxBlood);
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
        currentHealth -= damage;
        Debug.Log("HealthRemaining: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    // BLOOD (WIP)
    // Get it from enemies, you lose it with time
    // When it is at max you temporarily stop losing it (this will most likely need QA)
    public void GainBlood(float gain) //use instead of blood += n as it is likely that gaining blood will cause additional effects in the future.
    {
        if (bloodOverflow == true)
        {
            bloodOverflowTimeIncrease = 1; //very rough, QA will have to decide 
        }
        currentBlood += gain;
        if (currentBlood > maxBlood)
        {
            currentBlood = Mathf.Clamp(currentBlood, 0, maxBlood);
            StartCoroutine(BloodOverflow(1));
        }
        Debug.Log("Gained blood: " + gain);
    }
    IEnumerator BloodOverflow(float time) //temporalily disable blood deterioration
    {

        bloodOverflow = true;
        bloodSlider.fillRect.GetComponent<Image>().color = Color.black; //change fill area color to black to signal BloodOverFlow
        float timer = time;
        while (timer > 0f) //use bloodOverflowTimeIncrease to increase its time in different places (like GainBlood(float gain))
        {
            timer -= Time.deltaTime;
            if (bloodOverflowTimeIncrease > 0)
            {
                timer += bloodOverflowTimeIncrease;
                bloodOverflowTimeIncrease = 0;
            }
            yield return null;
        }
        bloodOverflow = false;
        bloodSlider.fillRect.GetComponent<Image>().color = Color.red;
    }
}
