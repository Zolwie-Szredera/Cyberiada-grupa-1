using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapons : MonoBehaviour
{
    [Header("stats")]
    public int damage;
    public float attackSpeed; //in seconds: how often can you attack
    public Transform attackOrigin; //where the attack begins: where the attack animation should begin
    protected float attackCooldown; //in seconds during gameplay: when can you attack again
    protected LayerMask damageableLayers;
    protected bool isHoldingAttackButton;
    public virtual void Start()
    {
        damageableLayers = LayerMask.GetMask("Enemy", "Destructible");
    }
    public virtual void Update()
    {
        if(attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        } else if(isHoldingAttackButton)
        {
            BasicAttack();
        }
    }
    public virtual void OnAttack(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            isHoldingAttackButton = true;
        } else if(context.canceled)
        {
            isHoldingAttackButton = false;
        }
    }
    public virtual void BasicAttack()
    {
        Debug.LogWarning("You forgot to override BasicAttack"); //this must be overriden in every weapon. I can't use "abstract" because reasons
    }
}
