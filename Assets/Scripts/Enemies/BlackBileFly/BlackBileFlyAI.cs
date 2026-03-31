using UnityEngine;

[RequireComponent(typeof(BlackBileFlyAttack))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(EnemyHoverPointFly))]
public class BlackBileFlyAI : Enemy
{
    [Header("AI settings")]
    public float firstShotDelay = 0.5f; //delay after spawning before first shot 
    public float hoverPointInterval = 3f; //in seconds: how often to pick a new hover point
    [Header("Transformation")]
    public GameObject transformedEnemyPrefab;
    public Animator animator;
    public float chanceToTransform;
    public float movementSpeedDuringTransform = 0.5f;
    private bool playerInRange;
    private bool firstShotDone;
    private float firstShotTimer;
    private float hoverPointTimer;
    private Vector2 currentHoverPoint;
    private BlackBileFlyAttack attackScript;
    private AudioSource transformAudioSource;
    private EnemyHoverPointFly flyScript;
    private State currentState;
    enum State
    {
        Idle,
        Moving,
        Attacking,
        Transforming
    }

    public override void Start()
    {
        base.Start();
        attackScript = GetComponent<BlackBileFlyAttack>();
        currentState = State.Idle;
        flyScript = GetComponent<EnemyHoverPointFly>();
        hoverPointTimer = hoverPointInterval;
        currentHoverPoint = flyScript.GetHoverPoint((Vector2)playerLocation.position);
        transformAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        bool isInRange = distanceToPlayer <= attackScript.attackRange;
        if (!playerInRange && isInRange)
        {
            playerInRange = true;
            firstShotTimer = firstShotDelay;
            return;
        }

        if (!firstShotDone)
        {
            firstShotTimer -= Time.deltaTime;
            if (firstShotTimer <= 0f)
            {
                currentState = State.Attacking;
                firstShotDone = true;
            }
            return;
        }

        // After first shot: periodically pick a new hover point
        if (currentState == State.Moving)
        {
            hoverPointTimer -= Time.deltaTime;
            if (hoverPointTimer <= 0f)
            {
                currentHoverPoint = flyScript.GetHoverPoint((Vector2)playerLocation.position);
                hoverPointTimer = hoverPointInterval;
            }
            flyScript.MoveTowards(currentHoverPoint);
            if (distanceToPlayer < attackScript.attackRange && attackCooldown <= 0f)
            {
                currentState = State.Attacking;
            }
        }
        if (currentState == State.Attacking)
        {
            animator.SetBool("isAttacking", true);
            attackCooldown = attackSpeed;
            currentState = State.Moving;
            hoverPointTimer = hoverPointInterval;
            currentHoverPoint = flyScript.GetHoverPoint((Vector2)playerLocation.position);
        } else
        {
            animator.SetBool("isAttacking", false);
        }
        if(currentState == State.Transforming)
        {
            //fly directly towards player during transform, but slower than usual
            flyScript.MoveTowards((Vector2)playerLocation.position);
        }
    }
    public override void Die()
    {
        transformAudioSource.Play();
        float chance = Random.Range(0f,1f);
        invulnerable = true; //prevent taking damage during transform
        if(chance <= chanceToTransform)
        {
            animator.SetBool("deathTransform", true);
            currentState = State.Transforming;
            movementSpeed = movementSpeedDuringTransform;
        } else
        {
            animator.SetBool("explosiveTransform", true);
            currentState = State.Transforming;
        }
    }
    public void TransfurComplete() //animator calls this after transform animation is finished
    { //I can't contain the silly
        GameObject newEnemy = Instantiate(transformedEnemyPrefab, transform.position, transform.rotation);
        if(spawner != null)
        {
            newEnemy.GetComponent<Enemy>().spawner = spawner;
        }
        Destroy(gameObject);
    }
    public void ExplosiveTransfur()
    { //transfur into a bomb lol
        attackScript.Explode();
        base.Die();
    }
}
