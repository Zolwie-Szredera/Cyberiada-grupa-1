using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponOrbit : MonoBehaviour
{
    [Header("Orbit Settings")]
    [Tooltip("Distance from player center")]
    public float orbitRadius = 1.5f;
    
    [Tooltip("How fast weapon rotates around player to follow mouse (0 = instant, higher = smoother)")]
    [Range(0f, 30f)]
    public float angleFollowSpeed = 15f;
    
    [Header("Rotation Settings")]
    [Tooltip("Offset applied to final rotation (degrees). Adjust if weapon points wrong direction.")]
    public float rotationOffset;
    
    [Header("Sprite Flipping")]
    [Tooltip("Auto-flip sprite when aiming left (prevents upside-down weapon)")]
    public bool enableSpriteFlipping = true;

    // Internal references
    private Transform playerTransform;
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    
    // State
    private float currentAngle;
    private float targetAngle;

    void Start()
    {
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError($"[WeaponOrbit] {gameObject.name}: Player not found! Tag required: 'Player'");
            enabled = false;
            return;
        }

        // Get camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError($"[WeaponOrbit] {gameObject.name}: Main Camera not found!");
            enabled = false;
            return;
        }

        // Get sprite renderer (optional, for flipping)
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null && enableSpriteFlipping)
        {
            Debug.LogWarning($"[WeaponOrbit] {gameObject.name}: No SpriteRenderer - flipping disabled.");
            enableSpriteFlipping = false;
        }

        // Initialize angle at 0 (right side)
        currentAngle = 0f;
        targetAngle = 0f;
    }

    void Update()
    {
        if (playerTransform == null || mainCamera == null)
            return;

        // Get mouse position in world space
        Vector2 mouseWorldPos = GetMouseWorldPosition();
        
        // Calculate orbit position
        UpdateOrbitPosition(mouseWorldPos);
        
        // Update weapon rotation
        UpdateWeaponRotation(mouseWorldPos);
        
        // Handle sprite flipping
        if (enableSpriteFlipping && spriteRenderer != null)
        {
            UpdateSpriteFlipping();
        }
    }

    /// <summary>
    /// Get mouse position in world coordinates
    /// </summary>
    Vector2 GetMouseWorldPosition()
    {
        if (Mouse.current == null)
            return transform.position;
            
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }

    /// <summary>
    /// Calculate and update weapon position on orbit
    /// </summary>
    void UpdateOrbitPosition(Vector2 mouseWorldPos)
    {
        Vector2 playerPos = playerTransform.position;
        
        // Calculate target angle from player to mouse
        Vector2 directionToMouse = mouseWorldPos - playerPos;
        targetAngle = Mathf.Atan2(directionToMouse.y, directionToMouse.x);
        
        // Smooth angle transition (optional)
        if (angleFollowSpeed > 0f)
        {
            // Convert to degrees for LerpAngle (handles wrapping)
            float currentDeg = currentAngle * Mathf.Rad2Deg;
            float targetDeg = targetAngle * Mathf.Rad2Deg;
            float smoothedDeg = Mathf.LerpAngle(currentDeg, targetDeg, angleFollowSpeed * Time.deltaTime);
            currentAngle = smoothedDeg * Mathf.Deg2Rad;
        }
        else
        {
            currentAngle = targetAngle;
        }
        
        // Calculate position on orbit using current angle
        // This ensures weapon ALWAYS stays on orbit circle, no lag
        Vector2 orbitOffset = new Vector2(
            Mathf.Cos(currentAngle) * orbitRadius,
            Mathf.Sin(currentAngle) * orbitRadius
        );
        
        // Apply position directly - no lerp, weapon sticks to orbit
        transform.position = playerPos + orbitOffset;
    }

    /// <summary>
    /// Rotate weapon to point at mouse
    /// </summary>
    void UpdateWeaponRotation(Vector2 mouseWorldPos)
    {
        // Direction from weapon to mouse
        Vector2 direction = mouseWorldPos - (Vector2)transform.position;
        
        // Calculate rotation angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Apply rotation with offset
        transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);
    }

    /// <summary>
    /// Flip sprite based on aim direction
    /// Simple: if aiming left (angle between 90 and 270), flip Y-axis
    /// </summary>
    void UpdateSpriteFlipping()
    {
        // Convert angle to degrees and normalize to -180 to 180
        float angleDegrees = currentAngle * Mathf.Rad2Deg;
        while (angleDegrees > 180f) angleDegrees -= 360f;
        while (angleDegrees < -180f) angleDegrees += 360f;
        
        // Determine if weapon is pointing left
        bool shouldFlip = angleDegrees > 90f || angleDegrees < -90f;
        
        // Flip sprite on Y-axis when pointing left
        // This prevents weapon from being upside-down
        spriteRenderer.flipY = shouldFlip;
    }

    /// <summary>
    /// Get current orbit angle in degrees (useful for other scripts)
    /// </summary>
    public float GetCurrentAngle()
    {
        return currentAngle * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Manually set orbit angle
    /// </summary>
    public void SetOrbitAngle(float angle)
    {
        currentAngle = angle;
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        if (playerTransform != null)
        {
            // Draw orbit circle
            Gizmos.color = Color.yellow;
            DrawCircle(playerTransform.position, orbitRadius, 36);
            
            // Draw line to weapon
            Gizmos.color = Color.red;
            Gizmos.DrawLine(playerTransform.position, transform.position);
            Gizmos.DrawWireSphere(transform.position, 0.1f);
        }
    }

    void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0f, 0f);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0f
            );
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}

