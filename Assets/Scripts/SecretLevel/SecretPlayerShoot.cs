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

    // --- NOWE ZMIENNE ---
    [Header("Upgrade Settings")]
    public int upgradeLevel = 1; // Startujemy od 1 kuli
    public float spreadAngle = 15f; // K¹t rozproszenia kul
    // --------------------

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
        if (context.started) isShooting = true;
        if (context.canceled) isShooting = false;
    }

    public void ProjectileAttack(Vector2 direction)
    {
        // Obliczamy bazowy k¹t w stopniach
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Pêtla tworz¹ca tyle kul, ile mamy upgrade'ów
        for (int i = 0; i < upgradeLevel; i++)
        {
            // Obliczanie przesuniêcia k¹towego, aby kule lecia³y wachlarzem
            float offset = (i - (upgradeLevel - 1) / 2f) * spreadAngle;
            Quaternion rotation = Quaternion.Euler(0, 0, baseAngle + offset);

            // Obliczanie nowego kierunku na podstawie rotacji
            Vector2 newDir = rotation * Vector2.right;

            GameObject currentProjectile = Instantiate(projectilePrefab, attackPoint.position, rotation);
            SecretProjectile proj = currentProjectile.GetComponent<SecretProjectile>();

            if (proj != null)
            {
                proj.Initiate(1, 100, projectileSpeed, newDir);
                proj.IgnoreParentObject(gameObject);
            }
        }
    }

    // Metoda wywo³ywana, gdy gracz dostanie obra¿enia
    public void LoseUpgrade()
    {
        if (upgradeLevel > 1)
        {
            upgradeLevel--;
            Debug.Log("Stracono upgrade! Poziom: " + upgradeLevel);
        }
    }
}