using UnityEngine;

[CreateAssetMenu(fileName = "LilyOfShadows", menuName = "Accessories/Charm/Lily Of Shadows")]
public class LilyOfShadows : Accessory
{
    [Header("Charm Stats")]
    public float dashBonus = 15f;

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(displayName))
            displayName = "Lily Of Shadows";

        description = $"+{dashBonus} dash force bonus";
    }

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