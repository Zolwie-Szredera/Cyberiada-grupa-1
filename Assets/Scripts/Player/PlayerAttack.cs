using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{

    [SerializeField] private Transform attackTransform;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask attackLayer;
    private RaycastHit2D[] hits;

    public int damageMelee;
    private InputAction attack;
    
    /*
     * v Weapons to add v
     * public int damageLongShot, damageBuuzSaw;
     * v Weapon cycle v
     * public int currentWeapon;
     */
    private void Start()
    {
        attack = new InputAction("Attack", InputActionType.Button, "<Mouse>/leftButton");
        attack.Enable();
    }
    void Update()
    {
        if(Input.GetButtonDown("Attack")) //michix bby girl fix it pls
        {
            AttackMeleeBasic();
        }
    }
    private void AttackMeleeBasic()
    {
        hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackLayer);
        for (int i = 0; i < hits.Length; i++)
        {
            Enemy enemy = hits[i].collider.gameObject.GetComponent<Enemy>();

            if (enemy != null )
            {
                enemy.TakeDamage(damageMelee);

            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    }
}
