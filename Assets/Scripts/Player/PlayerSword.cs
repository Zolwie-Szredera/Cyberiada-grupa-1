using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSword : PlayerWeapons
{
    [Header("Sword stats")]
    public float attackRange;
    public override void Update()
    {
        base.Update();
    }
    public override void BasicAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackOrigin.position, attackRange, damageableLayers);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
                Debug.Log("Hit an enemy: " + hit.name);
            }
            //if(hit.TryGetComponent(out Destructible destructible))
            //{
            //    destructible.TakeDamage(damage);
            //}
            //ADD DESTRUCTIBLES LATER
        }
        attackCooldown = attackSpeed;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackOrigin.position, attackRange);
    }
}
