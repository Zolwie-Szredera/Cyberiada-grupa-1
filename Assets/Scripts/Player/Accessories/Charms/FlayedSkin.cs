using UnityEngine;

[CreateAssetMenu(fileName = "FlayedSkin", menuName = "Accessories/Flayed Skin")]
public class FlayedSkin : Accessory
{
    private void OnEnable()
    {
        displayName = "Flayed skin";
        description = "If using a blood spell would kill you, leave you at 1 HP instead.";
    }

    public override void Apply(PlayerStats stats)
    {
        PlayerHealth playerHealth = stats.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogWarning($"[{Name}] Could not find PlayerHealth on the player.");
            return;
        }

        playerHealth.preventLethalBloodSpellCosts = true;
        Debug.Log($"[{Name}] Applied: blood spells can no longer kill you.");
    }

    public override void Remove(PlayerStats stats)
    {
        PlayerHealth playerHealth = stats.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogWarning($"[{Name}] Could not find PlayerHealth on the player.");
            return;
        }

        playerHealth.preventLethalBloodSpellCosts = false;
        Debug.Log($"[{Name}] Removed: blood spells can kill you again.");
    }
}