using UnityEngine;

public class BloodthirstyCharm : Accessory
{
    public float hpBonus = 20f;
    public int swordDamageBonus = 1;

    public BloodthirstyCharm()
    {
        name = "Bloodthirsty Charm";
        description = $"+{hpBonus} Max Blood, +{swordDamageBonus} Sword Damage";
    }

    public override void Apply(PlayerStats stats)
    {
        stats.maxBlood += hpBonus;
        stats.swordDamage += swordDamageBonus;
        Debug.Log($"[{name}] Applied: +{hpBonus} HP, +{swordDamageBonus} Sword Damage");
    }

    public override void Remove(PlayerStats stats)
    {
        stats.maxBlood -= hpBonus;
        stats.swordDamage -= swordDamageBonus;
        Debug.Log($"[{name}] Removed: -{hpBonus} HP, -{swordDamageBonus} Sword Damage");
    }
}
