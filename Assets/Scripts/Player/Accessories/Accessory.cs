using UnityEngine;

public abstract class Accessory : ScriptableObject
{
    public int slot; // 0 - head, 1 - body, 2 - arms, 3 - legs, 4 - all (for no accossory)
    //I don't think this int will be useful for anything besdides identifying the type of accessory
    public abstract void Apply();
    public abstract void Remove();
}
