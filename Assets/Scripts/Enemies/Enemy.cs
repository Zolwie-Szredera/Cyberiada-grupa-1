using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int hp;
    public int damage;
    public float movementSpeed;
    public float attackSpeed;
    
    public virtual void TakeDamage(int damageTaken)
    {
        hp -= damageTaken;
        if(hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
