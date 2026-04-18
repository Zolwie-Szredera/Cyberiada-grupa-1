using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarHandler : MonoBehaviour
{
    public GameObject healthBar;
    public TextMeshProUGUI text;
    public Image image;
    private Enemy boss = null;
    private int maxBossHealth;
    private int currentBossHealth;
    private bool inBattle = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthBar.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(boss != null)
        {
            currentBossHealth = boss.hp;
            image.fillAmount = (float)currentBossHealth / maxBossHealth;
        }
    }
    public void InitiateBossBattle(Enemy boss)
    {
        if(inBattle)
        {
            Debug.LogError("Tried to iniliate boss battle while one was already active");
        }
        inBattle = true;
        healthBar.SetActive(true);
        text.text = boss.gameObject.name;
        maxBossHealth = boss.hp;
        this.boss = boss;
    }
    public void EndBossBattle()
    {
        image.fillAmount = 1;
        healthBar.SetActive(false);
        boss = null;
        inBattle = false;
    }
}
