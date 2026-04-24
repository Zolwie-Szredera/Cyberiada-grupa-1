using UnityEngine;
using UnityEngine.InputSystem;

public class SecretPlayerShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float shootCooldown;
    public float projectileSpeed;

    private float shootTimer;
    private Vector2 mousePosition;

    void Start()
    {
        shootTimer = shootCooldown;
    }

    void Update()
    {
        shootTimer -= Time.deltaTime;

        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    public void OnShoot()
    {
        if (shootTimer <= 0)
        {
            Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;

            ProjectileAttack(direction);
            shootTimer = shootCooldown;
        }
    }

    public void ProjectileAttack(Vector2 direction)
    {
        GameObject currentProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        Projectile proj = currentProjectile.GetComponent<Projectile>();
        proj.Initiate(1, 100, projectileSpeed, direction);
        proj.IgnoreParentObject(gameObject);
    }
}