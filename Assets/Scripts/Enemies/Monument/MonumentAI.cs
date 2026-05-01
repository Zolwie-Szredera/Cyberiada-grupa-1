using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class MonumentAI : Enemy
{
    private static readonly int AttackRangedHash = Animator.StringToHash("AttackLaser");
    private static readonly int AttackRangedMoveHash = Animator.StringToHash("AttackShootWalk");
    private static readonly int AttackHash = Animator.StringToHash("AttackMelee");
    private static readonly int WalkHash = Animator.StringToHash("Walk");
    private static readonly int IdleHash = Animator.StringToHash("Idle");
    //animations: attack melee, attack ranged with movement, walk, attack ranged without movement

    enum State
    {
        Idle,
        Walk, //and try to attack if in range
        AttackLaser, //and not moving
        AttackShootWalk, //shoot and walk, try to stay in ranged range
        AttackMelee, //and not moving
    }
    [Header("Monument")]
    public float meleeRange; //melee player if in range
    public float tryToMeleeRange; //walk to player if in range
    public float rangedRange; //shoot if in range - laser or projectile depends on roll
    public float LaserAttackChance; //chance to shoot laser instead of walk and shoot when player is in ranged range
    public GameObject[] goopPoints;
    private TilemapEffectsHandler effectsHandler;
    private State currentState;
    private Animator animator;
    private bool blockStateChange = false;
    public override void Start()
    {
        base.Start();
        effectsHandler = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TilemapEffectsHandler>();
        animator = GetComponent<Animator>();
    }
    public void Update()
    {
        //STATE BEHAVIOUR
        if (currentState == State.Walk || currentState == State.AttackShootWalk)
        {
            WalkToPlayer(1);
        }
        
        //STATE TRANSITIONS
        if (blockStateChange) return;

        State nextState;

        if (distanceToPlayer <= meleeRange)
        {
            nextState = State.AttackMelee;
        }
        else if (distanceToPlayer <= tryToMeleeRange)
        {
            nextState = State.Walk;
        }
        else if (distanceToPlayer <= rangedRange)
        {
            float roll = Random.value;
            nextState = roll >= LaserAttackChance ? State.AttackShootWalk : State.AttackLaser;
        }
        else
        {
            nextState = State.Idle;
        }
        SetState(nextState);

        //most of the stuff like "instatiate projectiles" is handled via animator
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isGrounded)
        {
            foreach (GameObject point in goopPoints)
            {
                effectsHandler.PlaceGoop(point.transform.position);
            }
        }
    }
    private void SetState(State newState)
    {
        if (currentState == newState || blockStateChange) return;

        currentState = newState;

        animator.ResetTrigger(AttackHash);
        animator.ResetTrigger(AttackRangedHash);
        animator.ResetTrigger(AttackRangedMoveHash);
        animator.ResetTrigger(WalkHash);
        animator.ResetTrigger(IdleHash);

        switch (currentState)
        {
            case State.Idle:
                animator.SetTrigger(IdleHash);
                blockFlip = false;
                break;

            case State.Walk:
                animator.SetTrigger(WalkHash);
                blockFlip = false;
                break;

            case State.AttackMelee:
                animator.SetTrigger(AttackHash);
                rb.linearVelocity = Vector2.zero;
                blockFlip = true;
                blockStateChange = true; //block state change until animation event calls ExitAttackd
                break;

            case State.AttackLaser:
                animator.SetTrigger(AttackRangedHash);
                blockFlip = true;
                blockStateChange = true; //block state change until animation event calls ExitAttack
                break;

            case State.AttackShootWalk:
                animator.SetTrigger(AttackRangedMoveHash);
                blockFlip = false;
                blockStateChange = true; //block state change until animation event calls ExitAttack
                //with the exception that if player gets too close, it will switch to melee attack
                break;
        }

        attackCooldown = attackSpeed;
    }
    public void ExitAttack() //animation event
    {
        blockStateChange = false;
        if(blockFlip) //temporarily allow flipping to face player after attack
        {
            blockFlip = false;
            FacePlayer();
            blockFlip = true;
        }
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        // Display current state above the enemy
        UnityEditor.Handles.Label(transform.position + Vector3.up * 6f, currentState.ToString());
    }
    public void OnDrawGizmosSelected()
    {
        // Draw melee range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
        // Draw try to melee range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, tryToMeleeRange);
        // Draw ranged range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangedRange);
    }
#endif
}
