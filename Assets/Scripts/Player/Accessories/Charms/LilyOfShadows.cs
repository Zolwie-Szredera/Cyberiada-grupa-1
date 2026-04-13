using UnityEngine;

public class LilyOfShadows : Accessory
{
    public float dashBonus = 15f;

    public LilyOfShadows()
    {
        name = "Lily Of Shadows";
        description = $"+{dashBonus} dash force bonus";
    }

    public override void Apply(PlayerStats stats)
    {
        PlayerStats.dashForce += dashBonus;
        Debug.Log($"[{name}] Applied: +{dashBonus}");
    }

    public override void Remove(PlayerStats stats)
    {
        PlayerStats.dashForce -= dashBonus;
        Debug.Log($"[{name}] Removed: -{dashBonus}");
    }
}