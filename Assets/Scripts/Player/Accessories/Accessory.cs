using UnityEngine;

public abstract class Accessory : ScriptableObject
{
    [Header("Accessory")]
    public string displayName = "Unknown Accessory";
    public string description = "";
    public Sprite icon;

    public string Name => string.IsNullOrEmpty(displayName) ? base.name : displayName;

    public abstract void Apply(PlayerStats stats);
    public abstract void Remove(PlayerStats stats);
}
