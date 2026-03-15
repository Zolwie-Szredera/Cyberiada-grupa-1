using UnityEngine;

public class FaceFlyAttack : EnemyMeelee
{
    public float blackBile = 1;
        public override void MeeleeAttack()
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
                playerHealth.GainBlackBile(blackBile);
                Debug.Log(gameObject.name + " hit the player");
            } //no friendly fire for this enemy
            //if(hit.TryGetComponent(out Destructible destructible))
            //{
            //    destructible.TakeDamage(damage);
            //}
            //ADD DESTRUCTIBLES LATER
        }
    }
}
