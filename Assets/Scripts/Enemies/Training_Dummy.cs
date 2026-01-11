using UnityEngine;

public class Training_Dummy : Enemy
{
    public override void TakeDamage(int damageTaken)
    {
        Debug.Log(damageTaken);
    }
}
