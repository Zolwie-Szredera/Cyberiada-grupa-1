using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class PlayerSword : PlayerWeapons
{
    //In playerStats:
    //int swordDamage
    [Header("Sword stats")]
    public float attackRange;
    private AudioSource audioSource;
    public override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
    }
    
    public override void BasicAttack() //with life steal
    {
        origin = GetAttackPosition(attackDistance);
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, attackRange, damageableLayers);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(PlayerStats.swordDamage);
                float bloodSteal = PlayerStats.swordDamage / 5.0f;
                player.GetComponent<PlayerHealth>().GainBlood(bloodSteal); //this is important. Current steal: 20%
                //Debug.Log("Hit an enemy: " + hit.name + " and gained blood: " + bloodSteal);
            }
            //if(hit.TryGetComponent(out Destructible destructible))
            //{
            //    destructible.TakeDamage(damage);
            //}
            //ADD DESTRUCTIBLES LATER
        }
        attackCooldown = PlayerStats.attackSpeed;
    }
    private void OnDrawGizmosSelected()
    {
        Vector2 attackPos = GetAttackPosition(attackDistance);

        // Draw attack range sphere (main attack box)
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // Semi-transparent red
        Gizmos.DrawSphere(attackPos, attackRange);
        
        // Draw wireframe outline
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, attackRange);
        
        // Draw attack origin point
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPos, 0.1f);
        
        // Draw line from weapon center to attack position
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, attackPos);
        
        // Draw weapon direction arrow (using player-to-mouse direction, same as GetAttackPosition)
        Gizmos.color = Color.magenta;
        Vector2 playerPos2 = player != null ? player.transform.position : transform.position;
        Vector2 direction = mousePosition - playerPos2;
        if (direction.sqrMagnitude < 0.0001f) direction = Vector2.right;
        direction.Normalize();
        Gizmos.DrawRay(transform.position, direction * (attackDistance + attackRange));
        
        // Draw label with attack range info
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(
            (Vector3)attackPos + Vector3.up * (attackRange + 0.3f),
            $"Sword Attack Range\nRadius: {attackRange:F2}m\nDistance: {attackDistance:F2}m\nDamage: {PlayerStats.swordDamage}",
            new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12
            }
        );
        #endif
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Vector2 attackPos = GetAttackPosition(attackDistance);
            
            // Show when on cooldown vs ready to attack
            bool isReady = attackCooldown <= 0;
            
            // Different color based on cooldown status
            Gizmos.color = isReady ? new Color(0f, 1f, 0f, 0.1f) : new Color(1f, 0f, 0f, 0.05f);
            Gizmos.DrawSphere(attackPos, attackRange);
            
            // Wireframe
            Gizmos.color = isReady ? Color.green : Color.red;
            Gizmos.DrawWireSphere(attackPos, attackRange);
        }
    }
    public void ApplyDamage() //&play sound
    {
        BasicAttack();
        audioSource.Play();
    }
    public override void HandleAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
            animator.SetBool("attack", true);
        else if (context.canceled)
            animator.SetBool("attack", false);
    }
    public override void ForceAttackStart()
    {
        animator.SetBool("attack", true);
    }
    public override void ForceAttackStop()
    {
        animator.SetBool("attack", false);
    }
}
