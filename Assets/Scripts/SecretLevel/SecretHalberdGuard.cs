using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class SecretHalberdEnemy : SecretEnemy
{
    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;

    [Header("References")]
    public Animator animator;

    private float attackTimer;
    private bool isAttacking;

    private LayerMask playerLayer;
    private float baseSpeed;

    void Start()
    {
        attackTimer = attackCooldown;
        playerLayer = LayerMask.GetMask("Player");
        baseSpeed = movementSpeed;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        bool playerInRange = Physics2D.OverlapCircle(
            attackPoint.position,
            attackRange,
            playerLayer
        );

        // 🔥 ATAK
        if (playerInRange && attackTimer <= 0f && !isAttacking)
        {
            EnterAttack();
            return;
        }

        // 🔥 RUCH (tylko gdy nie atakuje)
        if (!isAttacking)
        {
            MoveInLine();
            animator.SetBool("walk", true);
        }
    }

    void EnterAttack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        movementSpeed = 0f; // zatrzymaj się jak w Undertale
        animator.SetBool("walk", false);
        animator.SetTrigger("attack");
    }

    // 🔥 WYWOŁYWANE Z ANIMACJI
    public void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            playerLayer
        );

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<SecretPlayer>(out var player))
            {
                player.TakeDamage(damage);
                Debug.Log(name + " hit player for " + damage);
            }
        }
    }

    // 🔥 WYWOŁYWANE Z ANIMACJI
    public void OnEndAttackAnimation()
    {
        isAttacking = false;
        movementSpeed = baseSpeed;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}