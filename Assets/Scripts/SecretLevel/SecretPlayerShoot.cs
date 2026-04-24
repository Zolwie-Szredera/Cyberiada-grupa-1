using UnityEngine;
using UnityEngine.InputSystem;

public class SecretPlayerShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float shootCooldown;
    public float projectileSpeed;
    public Transform attackPoint;

    private float shootTimer;
    private Vector2 mousePosition;
    private bool isShooting = false;

    void Start()
    {
        shootTimer = shootCooldown;
    }

    void Update()
    {
        shootTimer -= Time.deltaTime;

        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        if (isShooting && shootTimer <= 0)
        {
            Vector2 direction = (mousePosition - (Vector2)attackPoint.position).normalized;

            ProjectileAttack(direction);
            shootTimer = shootCooldown;
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isShooting = true;
        }
        if (context.canceled)
        {
            isShooting = false;
        }
    }

    public void ProjectileAttack(Vector2 direction)
    {
        GameObject currentProjectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);

        SecretProjectile proj = currentProjectile.GetComponent<SecretProjectile>();
        proj.Initiate(1, 100, projectileSpeed, direction);
        proj.IgnoreParentObject(gameObject);
    }
}