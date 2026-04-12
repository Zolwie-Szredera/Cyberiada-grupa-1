using UnityEngine;

public abstract class Accessory
{
    public string name = "Unknown Accessory";
    public string description = "";
    public Sprite icon;

    public abstract void Apply(PlayerStats stats);
    public abstract void Remove(PlayerStats stats);
}
