using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRanged : PlayerWeapons
{
    public Projectile projectile;
    public float projectileSpeed;
    public float projectileTTL;
    public float bloodCost;
    private bool isAttacking;

    public PlayerRanged()
    {
        isRangedWeapon = true;
    }

    public override void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isAttacking = true;
        } else if(context.canceled)
        {
            isAttacking = false;
        }
    }
    public override void Start()
    {
        base.Start();
        attackCooldown = 0; //makes quickswapping possible. Hell yeah
    }
    public override void Update()
    {
        base.Update();
        attackCooldown -= Time.deltaTime;
        if(isAttacking && attackCooldown <= 0)
        {
            BasicAttack();
        }
    }
    public override void BasicAttack()
    {
        player.GetComponent<PlayerHealth>().TakeDamage(bloodCost);
        
        origin = GetAttackPosition(0f);
        
        Vector2 direction = GetAttackDirection();
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        Projectile currentProjectile = Instantiate(projectile, origin, rotation);
        currentProjectile.Initiate(PlayerStats.rangedDamage, projectileTTL, projectileSpeed, direction);
        currentProjectile.IgnoreParentObject(GameObject.FindGameObjectWithTag("Player"));
        attackCooldown = PlayerStats.attackSpeed;
    }
    public override void ForceAttackStart()
    {
        isAttacking = true;
        if (animator != null)
        {
            animator.SetBool("attack", true);
        }
    }
    public override void ForceAttackStop()
    {
        isAttacking = false;
        if (animator != null)
        {
            animator.SetBool("attack", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 attackPos = GetAttackPosition(0f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPos, 0.15f);

        Vector2 direction = GetAttackDirection();
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(attackPos, direction * 2f);

        #if UNITY_EDITOR
        UnityEditor.Handles.Label(
            attackPos + Vector2.up * 0.5f,
            $"Ranged\nDamage: {PlayerStats.rangedDamage}\nCost: {bloodCost}",
            new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10
            }
        );
        #endif
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Vector2 attackPos = GetAttackPosition(0f);
            bool isReady = attackCooldown <= 0;

            Gizmos.color = isReady ? Color.green : Color.red;
            Gizmos.DrawWireSphere(attackPos, 0.1f);
        }
    }
}
