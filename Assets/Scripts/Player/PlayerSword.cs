using Unity.Mathematics;
using UnityEngine;

public class PlayerSword : PlayerWeapons
{
    [Header("Sword stats")]
    public float attackRange;
    public override void BasicAttack() //with life steal
    {
        origin = attackOrigin.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, attackRange, damageableLayers);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
                float bloodSteal = damage / 2.0f;
                player.GetComponent<PlayerHealth>().GainBlood(bloodSteal); //this is important. Current steal: 50%
                Debug.Log("Hit an enemy: " + hit.name + " and gained blood: " + bloodSteal);
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
    public void ApplyDamage() //it has to be a seperate function to prevent name ambiguity
    {
        BasicAttack();
    }
}
