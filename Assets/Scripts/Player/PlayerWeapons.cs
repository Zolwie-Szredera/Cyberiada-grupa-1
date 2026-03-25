using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapons : MonoBehaviour
{
    public Animator animator;
    [Header("stats")]
    public int damage;
    public float attackSpeed; //in seconds: how often can you attack
    public Transform attackOrigin; //where the attack begins: where the attack animation should begin
    [Header("Attack Position")]
    [Tooltip("Distance from weapon to attack point (used if no attackOrigin assigned)")]
    public float attackDistance = 0.5f;
    protected float attackCooldown; //in seconds during gameplay: when can you attack again
    protected LayerMask damageableLayers;
    protected GameObject player;
    protected Vector2 mousePosition;
    protected Vector2 origin;
    protected GameManager gameManager;
    public virtual void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        damageableLayers = LayerMask.GetMask("Enemy", "Destructible");
    }
    public virtual void Update()
    {
        mousePosition = gameManager.mousePosition;
    }
    public virtual void OnAttack(InputAction.CallbackContext context)
    {
        if(context.started)
            animator.SetBool("attack", true);
        else if(context.canceled)
            animator.SetBool("attack", false);
    }
    public virtual void ForceAttackStart()
    {
        animator.SetBool("attack", true);
    }
    public virtual void ForceAttackStop()
    {
        animator.SetBool("attack", false);
    }
    public virtual void BasicAttack()
    {
        Debug.LogWarning("You forgot to override BasicAttack"); //this must be overriden in every weapon. I can't use "abstract" because reasons
    }

    protected Vector2 GetAttackPosition(float customAttackDistance = -1f)
    {
        if (attackOrigin != null && attackOrigin.parent == transform)
        {
            return attackOrigin.position;
        }

        Vector2 weaponPos = transform.position;
        Vector2 playerPos = player != null ? player.transform.position : weaponPos;
        Vector2 direction = mousePosition - playerPos;

        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector2.right;
        direction.Normalize();

        float distance = customAttackDistance >= 0f ? customAttackDistance : attackDistance;
        return weaponPos + direction * distance;
    }

    protected Vector2 GetAttackDirection()
    {
        Vector2 playerPos = player != null ? player.transform.position : transform.position;
        Vector2 direction = mousePosition - playerPos;

        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector2.right;
        direction.Normalize();

        return direction;
    }
}
