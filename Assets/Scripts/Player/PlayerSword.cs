using UnityEngine;

public class PlayerSword : PlayerWeapons
{
    [Header("Sword stats")]
    public float attackRange;
    public float knockbackForce = 15f; // ZMIANA
    public float knockbackForceUpward = 2f; // ZMIANA

    [Header("Attack Position")]
    [Tooltip("Distance from weapon center to attack point (along weapon direction)")]
    public float attackDistance = 0.5f;

    private PlayerHealth _playerHealth; // ZMIANA

    public override void Start() // ZMIANA (Public, aby pasowa³o do PlayerWeapons i naprawi³o CS0507)
    { // ZMIANA
        base.Start(); // ZMIANA (Zapobiega NullReferenceException w Update klasy bazowej)
        if (player != null) // ZMIANA
            _playerHealth = player.GetComponent<PlayerHealth>(); // ZMIANA
    } // ZMIANA

    private Vector2 GetAttackPosition()
    {
        if (attackOrigin != null && attackOrigin.parent == transform)
        {
            return attackOrigin.position;
        }

        Vector2 weaponPos = transform.position;
        Vector2 playerPos = player != null ? player.transform.position : weaponPos;
        Vector2 direction = (mousePosition - playerPos);
        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector2.right;
        direction.Normalize();

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

                // --- LOGIKA KNOCKBACK (Unity 6) ---
                if (hit.attachedRigidbody != null) // ZMIANA
                { // ZMIANA
                    Vector2 horizontalDir = ((Vector2)hit.transform.position - (Vector2)player.transform.position).normalized; // ZMIANA
                    Vector2 finalForce = new Vector2(horizontalDir.x * knockbackForce, knockbackForceUpward); // ZMIANA

                    hit.attachedRigidbody.linearVelocity = Vector2.zero; // ZMIANA (Naprawa CS0618)
                    hit.attachedRigidbody.AddForce(finalForce, ForceMode2D.Impulse); // ZMIANA
                } // ZMIANA

                float bloodSteal = damage / 5.0f;
                if (_playerHealth != null) // ZMIANA
                    _playerHealth.GainBlood(bloodSteal); // ZMIANA
            }
        }
        attackCooldown = attackSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 attackPos = GetAttackPosition();
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(attackPos, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPos, 0.1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, attackPos);
        Vector2 playerPos2 = player != null ? player.transform.position : transform.position;
        Vector2 direction = (mousePosition - playerPos2);
        if (direction.sqrMagnitude < 0.0001f) direction = Vector2.right;
        direction.Normalize();
        Gizmos.DrawRay(transform.position, direction * (attackDistance + attackRange));
#if UNITY_EDITOR
        UnityEditor.Handles.Label((Vector3)attackPos + Vector3.up * (attackRange + 0.3f), $"Sword Attack Range\nRadius: {attackRange:F2}m\nDistance: {attackDistance:F2}m\nDamage: {damage}", new GUIStyle() { normal = new GUIStyleState() { textColor = Color.white }, alignment = TextAnchor.MiddleCenter, fontSize = 12 });
#endif
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Vector2 attackPos = GetAttackPosition();
            bool isReady = attackCooldown <= 0;
            Gizmos.color = isReady ? new Color(0f, 1f, 0f, 0.1f) : new Color(1f, 0f, 0f, 0.05f);
            Gizmos.DrawSphere(attackPos, attackRange);
            Gizmos.color = isReady ? Color.green : Color.red;
            Gizmos.DrawWireSphere(attackPos, attackRange);
        }
    }

    public void ApplyDamage() { BasicAttack(); }
}