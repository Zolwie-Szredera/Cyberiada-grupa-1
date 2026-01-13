using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PLAYER SPRITE CONTROLLER - Simple sprite flipping based on mouse position
/// This handles ONLY the visual sprite direction, doesn't affect children or physics
/// 
/// Usage: Attach to GameObject with SpriteRenderer (player's visual sprite)
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerSpriteFlip : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The main player transform (usually parent). Leave empty to auto-detect.")]
    public Transform playerTransform;
    
    [Header("Settings")]
    [Tooltip("Should sprite flip based on mouse position?")]
    public bool flipWithMouse = true;
    
    [Tooltip("Flip horizontally (X) or vertically (Y)?")]
    public bool flipX = true;

    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private bool currentlyFacingRight = true;

    void Start()
    {
        // Get sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Auto-find player transform if not set
        if (playerTransform == null)
        {
            // Try parent first
            if (transform.parent != null)
            {
                playerTransform = transform.parent;
            }
            else
            {
                // Try finding by tag
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                }
            }
            
            if (playerTransform == null)
            {
                Debug.LogError($"[PlayerSpriteController] {gameObject.name}: Could not find player transform!");
                enabled = false;
                return;
            }
        }

        // Get camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError($"[PlayerSpriteController] {gameObject.name}: Main Camera not found!");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (!flipWithMouse || mainCamera == null || playerTransform == null)
        {
            return;
        }

        // Get mouse world position
        Vector2 mouseWorldPos = GetMouseWorldPosition();
        
        // Check if mouse is to the right or left of player
        bool mouseIsRight = mouseWorldPos.x > playerTransform.position.x;
        
        // Update sprite flip
        if (flipX)
        {
            spriteRenderer.flipX = !mouseIsRight; // Flip when mouse is on left
        }
        else
        {
            spriteRenderer.flipY = !mouseIsRight; // Flip Y when mouse is on left
        }
        
        currentlyFacingRight = mouseIsRight;
    }

    Vector2 GetMouseWorldPosition()
    {
        if (Mouse.current == null)
            return playerTransform.position;
            
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }

    /// <summary>
    /// Check if player is currently facing right
    /// </summary>
    public bool IsFacingRight()
    {
        return currentlyFacingRight;
    }

    /// <summary>
    /// Manually set facing direction
    /// </summary>
    public void SetFacingRight(bool facingRight)
    {
        currentlyFacingRight = facingRight;
        if (flipX)
        {
            spriteRenderer.flipX = !facingRight;
        }
        else
        {
            spriteRenderer.flipY = !facingRight;
        }
    }
}

