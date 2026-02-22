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
        
        // Check if player is flipped (scale.x < 0)
        float playerScaleX = playerTransform.localScale.x;
        bool isPlayerFlipped = playerScaleX < 0;
        
        // Calculate position on orbit using current angle
        Vector2 orbitOffset = new Vector2(
            Mathf.Cos(currentAngle) * orbitRadius,
            Mathf.Sin(currentAngle) * orbitRadius
        );
        
        // If player is flipped, we need to set local position (not world position)
        // because the parent scale will flip it
        if (isPlayerFlipped)
        {
            // When parent is flipped, use local position and flip X
            transform.localPosition = new Vector3(-orbitOffset.x, orbitOffset.y, 0f);
        }
        else
        {
            // Normal: use local position
            transform.localPosition = new Vector3(orbitOffset.x, orbitOffset.y, 0f);
        }
    }

    /// <summary>
    /// Rotate weapon to point at mouse
    /// </summary>
    void UpdateWeaponRotation(Vector2 mouseWorldPos)
    {
        // Calculate direction from PLAYER to mouse (not weapon to mouse)
        Vector2 playerPos = playerTransform.position;
        Vector2 direction = mouseWorldPos - playerPos;
        
        // Calculate rotation angle in world space
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Check if player is flipped
        float playerScaleX = playerTransform.localScale.x;
        bool isPlayerFlipped = playerScaleX < 0;
        
        if (isPlayerFlipped)
        {
            // When parent is flipped (scale.x < 0), we need to flip the X component of direction
            // This compensates for the parent's negative scale
            float flippedAngle = Mathf.Atan2(direction.y, -direction.x) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(0f, 0f, flippedAngle + rotationOffset);
        }
        else
        {
            // Normal rotation - use localRotation for consistency
            transform.localRotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);
        }
    }
    
    /// <summary>
    /// Flip sprite based on aim direction
    /// Prevents weapon sprite from being upside-down
    /// </summary>
    void UpdateSpriteFlipping()
    {
        // Get mouse position to determine flip direction
        Vector2 mouseWorldPos = GetMouseWorldPosition();
        Vector2 playerPos = playerTransform.position;
        Vector2 direction = mouseWorldPos - playerPos;
        
        // Calculate angle to mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Normalize to 0-360
        if (angle < 0) angle += 360f;
        
        // Check if player is flipped
        float playerScaleX = playerTransform.localScale.x;
        bool isPlayerFlipped = playerScaleX < 0;
        
        // Determine if we should flip the sprite
        // When pointing left (90-270 degrees), flip Y to prevent upside-down
        bool shouldFlipY = angle > 90f && angle < 270f;
        
        // If player is flipped, we need to invert this logic
        if (isPlayerFlipped)
        {
            shouldFlipY = !shouldFlipY;
        }
        
        spriteRenderer.flipX = false;
        spriteRenderer.flipY = shouldFlipY;
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

