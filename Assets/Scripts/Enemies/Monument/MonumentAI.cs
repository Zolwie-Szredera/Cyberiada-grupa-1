using System.Collections;
using UnityEngine;
[RequireComponent(typeof(MonumentMelee))]

[RequireComponent(typeof(MonumentShooter))]
[RequireComponent(typeof(Animator))]
public class MonumentAI : Enemy
{
    private static readonly int AttackRangedHash = Animator.StringToHash("attackRanged");
    private static readonly int AttackRangedMoveHash = Animator.StringToHash("attackRangedMove");
    private static readonly int AttackHash = Animator.StringToHash("attackMelee");
    //animations: attack melee, attack ranged with movement, walk, attack ranged without movement

    enum State
    {
        Idle,
        Walk, //and try to attack if in range
        AttackHeavyRanged, //and not moving
        AttackMelee, //and not moving
        AttackRangedAndMoving,

    }
    public float meleeRange; //melee player if in range
    public float tryToMeleeRange; //walk to player if in range
    public float rangedRange; //shoot if in range - heavy and sit or shoot and walk (randomly)
    public float heavyAttackChance;
    public GameObject[] goopPoints;
    private TilemapEffectsHandler effectsHandler;
    private State currentState;
    private MonumentMelee meleeScript;
    private MonumentShooter rangedScript;
    private Animator animator;
    private bool blockStateChange = false;
    public override void Start()
    {
        base.Start();
        effectsHandler = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TilemapEffectsHandler>();
        meleeScript = GetComponent<MonumentMelee>();
        rangedScript = GetComponent<MonumentShooter>();
        animator = GetComponent<Animator>();
    }
    public void Update()
    {
        //STATE MANAGEMENT
        if (!blockStateChange)
        {
            if (distanceToPlayer <= meleeRange)
            {
                //melee the player
                EnterAttackMelee();
            }
            else if (distanceToPlayer <= tryToMeleeRange)
            {
                //try to walk to player to melee him
                EnterWalk();
            }
            else if (distanceToPlayer <= rangedRange)
            {
                //try to shoot the player, maybe walk and shoot, maybe just shoot heavy
                EnterAttackRanged();
            }
            else
            {
                EnterIdle();
            }
        }

        //STATE BEHAVIOUR
        if(currentState == State.Walk || currentState == State.AttackRangedAndMoving)
        {
            WalkToPlayer(1);
        }
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
    private void EnterAttackMelee()
    {
        currentState = State.AttackMelee;
        animator.SetTrigger(AttackHash);
        rb.linearVelocity = Vector2.zero;
        blockFlip = true;
    }
    private void EnterAttackRanged()
    {
        attackCooldown = attackSpeed;
        blockFlip = false;

        float roll = Random.Range(0f, 1f);
        if (roll <= heavyAttackChance)
        {
            currentState = State.AttackHeavyRanged;
            animator.SetTrigger(AttackRangedMoveHash);
            blockFlip = true;
        } else
        {
            currentState = State.AttackRangedAndMoving;
            animator.SetTrigger(AttackRangedHash);
        }
    }
    private void EnterWalk()
    {
        currentState = State.Walk;
        blockFlip = false;
    }
    private void EnterIdle()
    {
        blockFlip = false;
        currentState = State.Idle;
    }
}
