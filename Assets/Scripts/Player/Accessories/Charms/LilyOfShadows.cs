using UnityEngine;

[CreateAssetMenu(fileName = "FilyOfShadows", menuName ="Accessories/Lily of Shadows")]
public class LilyOfShadows : Accessory
{
    public float dashBonus = 15f;
    public override void Apply(PlayerStats stats)
    {
        PlayerStats.dashForce += dashBonus;
        Debug.Log($"[{Name}] Applied: +{dashBonus}");
    }

    public override void Remove(PlayerStats stats)
    {
        PlayerStats.dashForce -= dashBonus;
        Debug.Log($"[{Name}] Removed: -{dashBonus}");
    }
}