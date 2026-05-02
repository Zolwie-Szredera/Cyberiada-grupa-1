using UnityEngine;

public class EnemyMeelee : MonoBehaviour
{
    public int damage;
    public float attackRange;
    public Transform attackPoint;
    protected LayerMask damageableLayers;
    public virtual void Start()
    {
        damageableLayers = LayerMask.GetMask("Player", "Enemy", "Destructible");
    }
    public virtual void MeeleeAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, damageableLayers);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) //don't hit yourself
            {
                continue;
            }
            if (hit.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damage);
                Debug.Log(gameObject.name + " hit the player");
            }
            if (hit.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
        }

        DestructibleTilemapUtility.DamageAt(attackPoint.position, attackRange);
    }
    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
