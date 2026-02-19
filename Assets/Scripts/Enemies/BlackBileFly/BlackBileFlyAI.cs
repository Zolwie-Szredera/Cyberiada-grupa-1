using UnityEngine;

[RequireComponent(typeof(BlackBileFlyAttack))]
public class BlackBileFlyAI : Enemy
{
    [Header("Shooting")]
    [SerializeField] private float firstShotDelay = 3f;
    private bool playerInRange;
    private bool firstShotDone;
    private float firstShotTimer;
    private BlackBileFlyAttack attackScript;

    public override void Start()
    {
        base.Start();
        attackScript = GetComponent<BlackBileFlyAttack>();
    }

    private void Update()
    {
        if (playerLocation == null) return;
        bool isInRange = distanceToPlayer <= attackScript.attackRange;

        //  RESET usunięty — enemy nie cofnie shrink ani liczby strzałów

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
                attackScript.ProjectileArcAttack(playerLocation.position);
                firstShotDone = true;
                attackCooldown = attackSpeed;
            }
            return;
        }
        if (attackCooldown <= 0f)
        {
            attackScript.ProjectileArcAttack(playerLocation.position);
            attackCooldown = attackSpeed;
        }
    }
    public override void Die()
    {
        attackScript.Explode();
        base.Die();
    }
}
