using UnityEngine;

public abstract class Accessory : ScriptableObject
{
    public string displayName = "Unknown Accessory";
    public string description = "";
    public Sprite icon;
    //if display name, show normal name
    public string Name => string.IsNullOrEmpty(displayName) ? name : displayName;
    public abstract void Apply(PlayerStats stats);
    public abstract void Remove(PlayerStats stats);
}
