using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerHealth : MonoBehaviour
{
    //In playerStats:
    //float maxBlood
    public bool isInvulnerable = false;
    private PlayerController playerController;
    public Image bloodFill;
    public Image blackBileFill;
    public Canvas deathScreen;
    public ParticleSystem bloodParticles;
    [HideInInspector] public float currentBlackBile;
    [HideInInspector] public float currentBlood;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        
        currentBlood = PlayerStats.maxBlood;
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

    private void UpdateHealthUI()
    {
        if (bloodFill != null)
        {
            bloodFill.fillAmount = PlayerStats.maxBlood > 0 ? currentBlood / PlayerStats.maxBlood : 0f;
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
            bloodFill.fillAmount = currentBlood / PlayerStats.maxBlood;
        }
        if (currentBlackBile > 0)
        {
            if (blackBileFill != null)
            {
                blackBileFill.fillAmount = currentBlackBile / PlayerStats.maxBlood;
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
        currentBlood = PlayerStats.maxBlood;
        currentBlackBile = 0f;
        UpdateHealthUI();
        if (blackBileFill != null)
        {
            blackBileFill.fillAmount = 0f;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;
        currentBlood -= damage;
        Debug.Log($"Player took {damage} damage. Current health: {currentBlood}/{PlayerStats.maxBlood}");

        UpdateHealthUI();
        
        if (bloodParticles != null)
        {
            bloodFill.fillAmount = currentBlood / PlayerStats.maxBlood;
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
        if (currentBlood > PlayerStats.maxBlood || currentBlackBile != 0)
        {
            currentBlood = Mathf.Clamp(currentBlood, 0, PlayerStats.maxBlood - currentBlackBile);
        }
        UpdateHealthUI();
    }

    public void GainBlackBile(float amount)
    {
        currentBlackBile += amount;
        currentBlackBile = Mathf.Clamp(currentBlackBile, 0f, PlayerStats.maxBlood - currentBlood);
    }
}
