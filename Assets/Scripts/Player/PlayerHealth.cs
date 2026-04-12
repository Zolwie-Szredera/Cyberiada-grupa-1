using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealth : MonoBehaviour
{
    private PlayerStats playerStats;
    private PlayerController playerController;
    private float maxBlood;
    public float MaxBlood => maxBlood;
    public Image bloodFill;
    public Image blackBileFill;
    public Canvas deathScreen;
    public ParticleSystem bloodParticles;
    [HideInInspector] public float currentBlackBile;
    [HideInInspector] public float currentBlood;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerStats = GetComponent<PlayerStats>();
        
        if (playerStats != null)
        {
            maxBlood = PlayerStats.maxBlood;
            playerStats.OnStatsChanged += OnStatsChanged;
        }
        else
        {
            Debug.LogWarning("[PlayerHealth] PlayerStats not found! Using default max blood.");
            maxBlood = 100f;
        }
        
        currentBlood = maxBlood;
        currentBlackBile = 0;

        UpdateHealthUI();

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

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnStatsChanged -= OnStatsChanged;
        }
    }

    private void OnStatsChanged()
    {
        if (playerStats == null) return;
        
        float previousMaxBlood = maxBlood;
        maxBlood = PlayerStats.maxBlood;
        
        currentBlood = Mathf.Clamp(currentBlood, 0f, maxBlood);
        
        if (Mathf.Approximately(previousMaxBlood, maxBlood) == false)
        {
            Debug.Log($"[PlayerHealth] Max blood changed: {previousMaxBlood} -> {maxBlood}");
        }
        
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (bloodFill != null)
        {
            bloodFill.fillAmount = maxBlood > 0 ? currentBlood / maxBlood : 0f;
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
        if (currentBlackBile > 0)
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
        deathScreen.gameObject.SetActive(true);
        Debug.Log("You died! Click R to respawn at the last checkpoint.");
    }

    public void RestoreToMax()
    {
        currentBlood = maxBlood;
        currentBlackBile = 0f;
        UpdateHealthUI();
        if (blackBileFill != null)
        {
            blackBileFill.fillAmount = 0f;
        }
    }

    public void TakeDamage(float damage)
    {
        currentBlood -= damage;
        Debug.Log($"Player took {damage} damage. Current health: {currentBlood}/{maxBlood}");

        UpdateHealthUI();
        
        if (bloodParticles != null)
        {
            ParticleSystem ps = Instantiate(bloodParticles, transform.position, Quaternion.identity);
            ps.Play();
        }
        if (currentBlood <= 0)
        {
            Die();
        }
    }

    public void GainBlood(float gain)
    {
        currentBlood += gain;
        if (currentBlood > maxBlood || currentBlackBile != 0)
        {
            currentBlood = Mathf.Clamp(currentBlood, 0, maxBlood - currentBlackBile);
        }
        UpdateHealthUI();
    }

    public void GainBlackBile(float amount)
    {
        currentBlackBile += amount;
        currentBlackBile = Mathf.Clamp(currentBlackBile, 0f, maxBlood - currentBlood);
    }
}
