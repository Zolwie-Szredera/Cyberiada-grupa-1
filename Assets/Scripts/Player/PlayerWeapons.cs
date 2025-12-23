using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapons : MonoBehaviour
{
    public Animator animator;
    [Header("stats")]
    public int damage;
    public float attackSpeed; //in seconds: how often can you attack
    public Transform attackOrigin; //where the attack begins: where the attack animation should begin
    protected float attackCooldown; //in seconds during gameplay: when can you attack again
    protected LayerMask damageableLayers;
    public virtual void Start()
    {
        damageableLayers = LayerMask.GetMask("Enemy", "Destructible");
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            animator.SetBool("attack", true);
        } else if(context.canceled)
        {
            animator.SetBool("attack", false);
        }
    }
    public virtual void BasicAttack()
    {
        Debug.LogWarning("You forgot to override BasicAttack"); //this must be overriden in every weapon. I can't use "abstract" because reasons
    }
}
