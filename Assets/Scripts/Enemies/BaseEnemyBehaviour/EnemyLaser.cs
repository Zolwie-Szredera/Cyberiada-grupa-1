using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    public float damage;
    public float damageInterval; //how often the laser applies damage
    public float rotationSpeed; //how fast the laser can rotate to follow the player
    public float startAngle; //the angle at which the laser starts, so it can rotate to follow the player
    public Transform startPoint;
    public LineRenderer laserRenderer;
    public ParticleSystem laserEffectPrefab; //on the ground
    private bool isShooting;
    private Transform target;
    private float damageTimer;
    private float currentAngle;
    void Start()
    {
        damageTimer = damageInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShooting) return;

        Vector2 startPos = startPoint.position;
        Vector2 targetDirection = (Vector2)target.position - startPos;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        Vector2 direction = new(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));
        RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, direction.normalized, 100f, LayerMask.GetMask("Player", "Ground", "Enemy", "StickyWall"));
        RaycastHit2D hit = default;
        Debug.DrawRay(startPos, direction.normalized * 100f, Color.red);
        if (hits.Length == 0)
        {
            //Debug.LogWarning("Laser is shooting but not hitting anything? This should not happen.");
            Debug.DrawRay(startPos, direction.normalized * 100f, Color.red);
            foreach (var debughit in Physics2D.RaycastAll(startPos, direction, 100f))
            {
                Debug.Log(debughit.collider.name + " layer: " + LayerMask.LayerToName(debughit.collider.gameObject.layer));
            }
            return;
        }
        foreach (RaycastHit2D candidate in hits)
        {
            if (candidate.collider == null)
            {
                Debug.LogWarning("Raycast hit without collider? This should not happen.");
                continue;
            }
            if (candidate.collider.transform.root == transform.root)
                continue;
            hit = candidate;
            break;
        }

        if (hit.collider != null)
        {
            //visuals
            laserRenderer.SetPosition(0, startPos);
            laserRenderer.SetPosition(1, hit.point);
            float effectRotationZ = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg - 20f;
            Instantiate(laserEffectPrefab, hit.point, Quaternion.Euler(0f, 0f, effectRotationZ));

            damageTimer -= Time.deltaTime;
            if (hit.collider.CompareTag("Player") && damageTimer <= 0f)
            {
                hit.collider.GetComponent<PlayerHealth>().TakeDamage(damage);
                damageTimer = damageInterval;
                Debug.Log("Player hit by laser for " + damage + " damage.");
            }
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") && damageTimer <= 0f)
            {
                hit.collider.GetComponent<Enemy>().TakeDamage((int)damage);
                damageTimer = damageInterval;
                Debug.Log("Enemy hit by laser for " + damage + " damage.");
            }
        }
    }
    public void StartLaser(Transform target)
    {
        isShooting = true;
        this.target = target;
        Vector2 startPos = startPoint.position;
        Vector2 targetDirection = (Vector2)target.position - startPos;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        currentAngle = targetAngle + startAngle;
        laserRenderer.enabled = true;
    }
    public void StopLaser()
    {
        isShooting = false;
        laserRenderer.enabled = false;
    }
}
