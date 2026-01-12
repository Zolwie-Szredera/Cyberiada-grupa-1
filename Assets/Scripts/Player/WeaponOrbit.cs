using UnityEngine;

public class WeaponOrbit : MonoBehaviour
{
    [Header("Orbit Settings")]
    [Tooltip("Orbit radius around the player")]
    public float orbitRadius = 1.5f;
    
    [Tooltip("Should the weapon follow mouse position")]
    public bool followMousePosition = true;
    
    [Tooltip("Position adjustment speed (0 = instant, higher = smoother)")]
    [Range(0f, 20f)]
    public float positionSmoothSpeed = 10f;
    
    [Tooltip("Should the weapon automatically aim towards the mouse")]
    public bool rotateTowardsMouse = true;
    
    [Tooltip("Weapon rotation offset (in degrees)")]
    public float rotationOffset;

    private Transform playerTransform;
    private GameManager gameManager;
    private Vector2 currentOrbitPosition;
    private float currentOrbitAngle;

    void Start()
    {
        // Znajdź gracza
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("WeaponOrbit: Nie znaleziono gracza z tagiem 'Player'");
        }

        // Znajdź GameManager do śledzenia pozycji myszy
        GameObject gm = GameObject.FindGameObjectWithTag("GameManager");
        if (gm != null)
        {
            gameManager = gm.GetComponent<GameManager>();
        }
        else
        {
            Debug.LogError("WeaponOrbit: Nie znaleziono GameManager");
        }
        
        // Ustaw początkową pozycję
        if (playerTransform != null)
        {
            currentOrbitAngle = 0f;
        }
    }

    void Update()
    {
        if (playerTransform == null || gameManager == null) return;

        // Oblicz pozycję na orbicie
        CalculateOrbitPosition();

        // Ustaw pozycję broni
        transform.position = currentOrbitPosition;

        // Obróć broń w kierunku myszy
        if (rotateTowardsMouse)
        {
            RotateTowardsMouse();
        }
    }

    void CalculateOrbitPosition()
    {
        if (followMousePosition)
        {
            // Oblicz kąt od gracza do myszy
            Vector2 directionToMouse = gameManager.mousePosition - (Vector2)playerTransform.position;
            float targetAngle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
            
            // Płynne dostosowanie kąta (jeśli smoothSpeed > 0)
            if (positionSmoothSpeed > 0f)
            {
                currentOrbitAngle = Mathf.LerpAngle(currentOrbitAngle, targetAngle, positionSmoothSpeed * Time.deltaTime);
            }
            else
            {
                currentOrbitAngle = targetAngle;
            }
        }
        
        // Konwertuj kąt z stopni na radiany
        float angleInRadians = currentOrbitAngle * Mathf.Deg2Rad;

        // Oblicz pozycję na okręgu
        float x = playerTransform.position.x + Mathf.Cos(angleInRadians) * orbitRadius;
        float y = playerTransform.position.y + Mathf.Sin(angleInRadians) * orbitRadius;

        currentOrbitPosition = new Vector2(x, y);
    }

    void RotateTowardsMouse()
    {
        // Oblicz kierunek od broni do myszy
        Vector2 direction = gameManager.mousePosition - (Vector2)transform.position;
        
        // Oblicz kąt rotacji
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Zastosuj rotację z offsetem
        transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);
    }

    // Funkcja pomocnicza do ustawiania kąta orbity z zewnątrz (dla trybu manualnego)
    public void SetOrbitAngle(float angle)
    {
        currentOrbitAngle = angle;
    }
    
    // Funkcja do pobierania aktualnego kąta
    public float GetCurrentAngle()
    {
        return currentOrbitAngle;
    }

    // Rysuj orbit w edytorze dla debugowania
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        if (playerTransform != null)
        {
            Gizmos.color = Color.yellow;
            // Rysuj okrąg orbity
            DrawCircle(playerTransform.position, orbitRadius, 36);
            
            // Rysuj linię do aktualnej pozycji broni
            Vector3 weaponPos = transform.position;
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(playerTransform.position, weaponPos);
            Gizmos.DrawWireSphere(weaponPos, 0.1f);
            
            // Rysuj kierunek do myszy (jeśli GameManager dostępny)
            if (gameManager != null)
            {
                Gizmos.color = Color.cyan;
                Vector3 mousePos = gameManager.mousePosition;
                mousePos.z = playerTransform.position.z;
                Gizmos.DrawLine(playerTransform.position, mousePos);
            }
        }
    }

    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}

