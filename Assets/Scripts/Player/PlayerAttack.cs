using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("stats")]
    public float attackRange = 1.5f;
    public int damageMelee;
    public float attackSpeed = 1.0f;

    [SerializeField] private Transform attackTransform;
    [SerializeField] private LayerMask attackLayer;
    private RaycastHit2D[] hits;
    private float nextAttackTime;
    private bool isHoldingAttackButton = false;
    /*
     * v Weapons to add v
     * public int damageLongShot, damageBuuzSaw;
     * v Weapon cycle v
     * public int currentWeapon;
     */
    void Update()
    {
        if(isHoldingAttackButton && Time.time > nextAttackTime)
        {
            AttackMeleeBasic();
            nextAttackTime = Time.time + attackSpeed;
        }
    }
    private void AttackMeleeBasic()
    {
        hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackLayer);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDamage(damageMelee);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            isHoldingAttackButton = true;
        } else if(context.canceled)
        {
            isHoldingAttackButton = false;
        }
    }
}
