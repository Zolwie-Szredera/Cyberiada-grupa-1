using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// WEAPON ORBIT SYSTEM - COMPLETE REWRITE
/// Clean, simple system for weapon positioning and rotation around player.
/// No complex parent flipping logic - just pure, elegant math.
/// 
/// How it works:
/// 1. Gets mouse position in world space
/// 2. Calculates angle from player to mouse
/// 3. Positions weapon on orbit at that angle
/// 4. Rotates weapon to point at mouse
/// 5. Flips sprite if needed (when aiming left)
/// </summary>
public class WeaponOrbit : MonoBehaviour
{
    [Header("Orbit Settings")]
    [Tooltip("Distance from player center")]
    public float orbitRadius = 1.5f;
    
    [Tooltip("How fast weapon moves to target position (0 = instant)")]
    [Range(0f, 30f)]
    public float positionSmoothSpeed = 10f;
    
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
    private Vector2 currentTargetPosition;
    private float currentAngle;

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

        // Initialize at player position
        currentTargetPosition = playerTransform.position;
        transform.position = currentTargetPosition;
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
        
        // Calculate angle from player to mouse
        Vector2 directionToMouse = mouseWorldPos - playerPos;
        float targetAngle = Mathf.Atan2(directionToMouse.y, directionToMouse.x);
        
        // Calculate target position on orbit
        Vector2 orbitOffset = new Vector2(
            Mathf.Cos(targetAngle) * orbitRadius,
            Mathf.Sin(targetAngle) * orbitRadius
        );
        Vector2 targetPosition = playerPos + orbitOffset;
        
        // Smooth movement or instant
        if (positionSmoothSpeed > 0f)
        {
            currentTargetPosition = Vector2.Lerp(
                currentTargetPosition,
                targetPosition,
                positionSmoothSpeed * Time.deltaTime
            );
        }
        else
        {
            currentTargetPosition = targetPosition;
        }
        
        // Apply position
        transform.position = currentTargetPosition;
        
        // Store angle for other uses
        currentAngle = targetAngle * Mathf.Rad2Deg;
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
        // Normalize angle to -180 to 180
        float normalizedAngle = currentAngle;
        while (normalizedAngle > 180f) normalizedAngle -= 360f;
        while (normalizedAngle < -180f) normalizedAngle += 360f;
        
        // Determine if weapon is pointing left
        bool shouldFlip = normalizedAngle > 90f || normalizedAngle < -90f;
        
        // Flip sprite on Y-axis when pointing left
        // This prevents weapon from being upside-down
        spriteRenderer.flipY = shouldFlip;
    }

    /// <summary>
    /// Get current orbit angle (useful for other scripts)
    /// </summary>
    public float GetCurrentAngle()
    {
        return currentAngle;
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

