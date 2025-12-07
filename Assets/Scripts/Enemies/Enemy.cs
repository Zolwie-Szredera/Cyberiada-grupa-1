using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp;
    public int damage;
    
    public virtual void TakeDamage(int damageTaken)
    {
        hp -= damageTaken;
        if(hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
