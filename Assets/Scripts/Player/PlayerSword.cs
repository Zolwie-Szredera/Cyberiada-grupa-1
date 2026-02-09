using UnityEngine;

public class PlayerSword : PlayerWeapons
{
    [Header("Sword stats")]
    public float attackRange;
    
    [Header("Attack Position")]
    [Tooltip("Distance from weapon center to attack point (along weapon direction)")]
    public float attackDistance = 0.5f;
    
    // Calculate actual attack position (follows weapon rotation)
    private Vector2 GetAttackPosition()
    {
        // If attackOrigin is assigned and is a child of this weapon, use it
        if (attackOrigin != null && attackOrigin.parent == transform)
        {
            return attackOrigin.position;
        }
        
        // Otherwise, calculate position in front of weapon based on rotation
        Vector2 weaponPos = transform.position;
        float weaponAngle = transform.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(weaponAngle), Mathf.Sin(weaponAngle));
        
        return weaponPos + direction * attackDistance;
    }
    
    public override void BasicAttack() //with life steal
    {
        origin = GetAttackPosition();
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
        // Calculate current attack position
        Vector2 attackPos = GetAttackPosition();

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
        
        // Draw weapon direction arrow
        Gizmos.color = Color.magenta;
        float weaponAngle = transform.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(weaponAngle), Mathf.Sin(weaponAngle));
        Gizmos.DrawRay(transform.position, direction * (attackDistance + attackRange));
        
        // Draw label with attack range info
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(
            (Vector3)attackPos + Vector3.up * (attackRange + 0.3f),
            $"Sword Attack Range\nRadius: {attackRange:F2}m\nDistance: {attackDistance:F2}m\nDamage: {damage}",
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
        // Always show attack box during play mode (lighter color)
        if (Application.isPlaying)
        {
            Vector2 attackPos = GetAttackPosition();
            
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
    public void ApplyDamage()
    {
        BasicAttack();
    }
}
