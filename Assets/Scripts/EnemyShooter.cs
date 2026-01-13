using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private ProjectileArc projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootCooldown = 1.5f;
    [SerializeField] private float shootRange = 50f;
    [SerializeField] private float firstShotDelay = 3f;

    [Header("Shrink Settings")]
    [SerializeField] private int maxShots = 5;
    [SerializeField, Range(0.1f, 1f)] private float finalScalePercent = 0.5f;

    [Header("Explosion")]
    [SerializeField] private ExplosionProjectile explosionProjectilePrefab;
    [SerializeField] private int explosionProjectileCount = 8;
    [SerializeField] private bool destroyAfterExplosion = true;

    private Transform player;
    private float timer;
    private int shotsFired;
    private Vector2 startScale;

    private bool playerInRange;
    private bool firstShotDone;
    private float firstShotTimer;
    private bool exploded;

    private void Start()
    {
        startScale = transform.localScale;
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool isInRange = distanceToPlayer <= shootRange;

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
                ShootNormal();
                firstShotDone = true;
                timer = shootCooldown;
            }
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ShootNormal();
            timer = shootCooldown;
        }
    }

    private void ShootNormal()
    {
        if (exploded) return;

        shotsFired++;

        if (shotsFired >= maxShots)
        {
            Explode();
            return;
        }

        ProjectileArc proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.Initialize(player);
        UpdateScale();
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        UpdateScale();

        float angleStep = 360f / explosionProjectileCount;

        for (int i = 0; i < explosionProjectileCount; i++)
        {
            float angle = i * angleStep;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);

            ExplosionProjectile p = Instantiate(explosionProjectilePrefab, transform.position, rot);
            p.Initialize(rot * Vector2.right);
        }

        if (destroyAfterExplosion)
        {
            Destroy(gameObject, 0.05f);
        }
    }

    private void UpdateScale()
    {
        float t = (float)shotsFired / maxShots;
        Vector2 targetScale = startScale * finalScalePercent;
        Vector2 newScale = Vector2.Lerp(startScale, targetScale, t);
        transform.localScale = new Vector3(newScale.x, newScale.y, 1f);
    }
}
