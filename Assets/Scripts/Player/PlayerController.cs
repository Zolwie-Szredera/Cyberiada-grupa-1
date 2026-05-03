using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    //In playerStats:
    //float MoveSpeed
    //float JumpForce
    //int AirJumps
    //float AccelerationRate
    //float DecelerationRate
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");

    private enum AttackOwner
    {
        None,
        Primary,
        Secondary
    }

    private Rigidbody2D rb;
    [HideInInspector] public Vector2 moveInput;
    [Header("UI")]
    public TextMeshProUGUI interactText;
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;
    [HideInInspector] public int remainingAirJumps;

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.55f;
    public LayerMask stickyWallLayer;
    private bool isTouchingWall; // with coyote time
    private bool wallCoyoteActive;
    public float wallCoyoteTime = 0.15f;
    private float wallCoyoteTimer;

    [Header("Platforms")]
    public LayerMask platformLayer;

    [Header("Animation")]
    public Animator animator;
    [HideInInspector] public bool isGrounded;

    private bool isWallJumping;
    private bool isJumping;
    private bool justWallJumped; // to prevent wasted air jumps
    private bool facingRight = true;
    private bool isHoldingDown;
    private bool isWalkingAnimated;
    private bool primaryAttackHeld;
    private bool secondaryAttackHeld;
    private AttackOwner activeAttackOwner;

    private GameManager gameManager;
    private PlayerControls controls;
    private WeaponsManager weaponsManager;
    private PlayerWeapons primaryWeapon;
    private PlayerWeapons secondaryWeapon;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        if (controls == null)
        {
            Debug.LogWarning("[PlayerController] controls was null in OnEnable, initializing now.");
            controls = new PlayerControls();
        }

        controls.Enable();
        controls.player.Attack.started += OnAttack;
        controls.player.Attack.canceled += OnAttack;
    }

    private void OnDisable()
    {
        controls.player.Attack.started -= OnAttack;
        controls.player.Attack.canceled -= OnAttack;
        controls.Disable();

        StopAllAttacks();
        primaryAttackHeld = false;
        secondaryAttackHeld = false;
        activeAttackOwner = AttackOwner.None;
    }

    private void Start()
    {
        remainingAirJumps = PlayerStats.isDoubleJumpUnlocked ? PlayerStats.airJumps : 0;
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }

        weaponsManager = FindAnyObjectByType<WeaponsManager>();
        CacheWeaponSlots();
    }
    private void CacheWeaponSlots()
    {
        if (weaponsManager == null)
        {
            Debug.LogWarning("[PlayerController] WeaponsManager not found.");
            return;
        }

        var weapon0 = weaponsManager.GetWeaponAt(0);
        var weapon1 = weaponsManager.GetWeaponAt(1);

        primaryWeapon = weapon0 != null ? weapon0.GetComponent<PlayerWeapons>() : null;
        secondaryWeapon = weapon1 != null ? weapon1.GetComponent<PlayerWeapons>() : null;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput.x > 0)
        {
            ChangeSpriteDirection(true);
        }
        else if (moveInput.x < 0)
        {
            ChangeSpriteDirection(false);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        bool canUseAirJump = PlayerStats.isDoubleJumpUnlocked && remainingAirJumps > 0;

        if (context.started && isTouchingWall)
        {
            isWallJumping = true;
        }
        else if (context.started && (isGrounded || canUseAirJump) && !justWallJumped)
        {
            isJumping = true;
        }
        else if (context.started)
        {
            Debug.Log("Prevented unnecessary air jump");
        }
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHoldingDown = true;
        }
        else if (context.canceled)
        {
            isHoldingDown = false;
        }
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer + platformLayer);
        if (isGrounded)
        {
            remainingAirJumps = PlayerStats.isDoubleJumpUnlocked ? PlayerStats.airJumps : 0;
        }

        WallCheck();

        float targetSpeed = moveInput.x * PlayerStats.moveSpeed;
        float velocityDifferenceX = targetSpeed - rb.linearVelocity.x;
        float maxSpeedChange = (Mathf.Abs(targetSpeed) > Mathf.Abs(rb.linearVelocity.x) ? PlayerStats.accelerationRate : PlayerStats.decelerationRate) * Time.deltaTime;
        float movementX = Mathf.Clamp(velocityDifferenceX, -maxSpeedChange, maxSpeedChange);
        rb.linearVelocity += new Vector2(movementX, 0f);

        HandleSecondaryAttack();

        if (isWallJumping)
        {
            rb.linearVelocity = Vector2.zero;
            int direction = 0;
            if (Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, stickyWallLayer))
            {
                direction = -1;
            }
            else if (Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, stickyWallLayer))
            {
                direction = 1;
            }
            rb.linearVelocity = new Vector2(direction * 7f, PlayerStats.jumpForce);
            isWallJumping = false;
            StartCoroutine(PreventAirJumpWaste(0.2f));
        }

        if (isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * PlayerStats.jumpForce, ForceMode2D.Impulse);

            if (!isGrounded && !isTouchingWall)
            {
                remainingAirJumps--;
            }
            isJumping = false;
        }

        if (isHoldingDown && isGrounded)
        {
            PlatformDrop();
        }

        if (rb.linearVelocity.y < -0.2f && rb.linearVelocity.y > -50.0f)
        {
            rb.linearVelocity += 1.5f * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocityX, Mathf.Clamp(rb.linearVelocity.y, -50f, 50f));

        bool shouldWalkAnimate = Mathf.Abs(rb.linearVelocity.x) > 0f;
        if (shouldWalkAnimate != isWalkingAnimated)
        {
            animator.SetBool(IsWalkingHash, shouldWalkAnimate);
            isWalkingAnimated = shouldWalkAnimate;
        }

        DrawDebugArrow();
    }

    private void HandleSecondaryAttack()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            secondaryAttackHeld = true;
            ResolveAttackOwner();
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            secondaryAttackHeld = false;
            ResolveAttackOwner();
        }
    }

    private void ResolveAttackOwner()
    {
        AttackOwner newOwner = activeAttackOwner;

        if (activeAttackOwner == AttackOwner.None)
        {
            if (primaryAttackHeld)
                newOwner = AttackOwner.Primary;
            else if (secondaryAttackHeld)
                newOwner = AttackOwner.Secondary;
        }
        else if (activeAttackOwner == AttackOwner.Primary && !primaryAttackHeld)
        {
            newOwner = secondaryAttackHeld ? AttackOwner.Secondary : AttackOwner.None;
        }
        else if (activeAttackOwner == AttackOwner.Secondary && !secondaryAttackHeld)
        {
            newOwner = primaryAttackHeld ? AttackOwner.Primary : AttackOwner.None;
        }

        if (newOwner == activeAttackOwner)
            return;

        switch (activeAttackOwner)
        {
            case AttackOwner.Primary:
                primaryWeapon?.ForceAttackStop();
                break;
            case AttackOwner.Secondary:
                secondaryWeapon?.ForceAttackStop();
                break;
        }

        activeAttackOwner = newOwner;

        switch (activeAttackOwner)
        {
            case AttackOwner.Primary:
                primaryWeapon?.ForceAttackStart();
                break;
            case AttackOwner.Secondary:
                secondaryWeapon?.ForceAttackStart();
                break;
        }
    }

    private void StopAllAttacks()
    {
        primaryWeapon?.ForceAttackStop();
        secondaryWeapon?.ForceAttackStop();
    }

    private void DrawDebugArrow()
    {
        if (gameManager == null) return;

        Vector3 start = transform.position;
        Vector3 mousePos = gameManager.mousePosition;
        mousePos.z = 0;
        start.z = 0;

        Vector3 direction = (mousePos - start).normalized;
        float length = 3f;
        Vector3 end = start + direction * length;
        Color color = Color.green;

        Debug.DrawLine(start, end, color);

        float arrowHeadSize = 0.5f;
        Vector3 right = Quaternion.Euler(0, 0, 135) * direction * arrowHeadSize;
        Vector3 left = Quaternion.Euler(0, 0, -135) * direction * arrowHeadSize;

        Debug.DrawRay(end, right, color);
        Debug.DrawRay(end, left, color);
    }

    void WallCheck()
    {
        bool wallContact = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, stickyWallLayer) ||
                           Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, stickyWallLayer);

        if (wallContact)
        {
            isTouchingWall = true;
            wallCoyoteActive = false;
            wallCoyoteTimer = 0f;
            return;
        }

        if (isTouchingWall && !wallCoyoteActive)
        {
            wallCoyoteActive = true;
            wallCoyoteTimer = wallCoyoteTime;
        }

        if (wallCoyoteActive)
        {
            wallCoyoteTimer -= Time.deltaTime;
            if (wallCoyoteTimer > 0f)
            {
                isTouchingWall = true;
            }
            else
            {
                isTouchingWall = false;
                wallCoyoteActive = false;
            }
        }
        else
        {
            isTouchingWall = false;
        }
    }

    IEnumerator PreventAirJumpWaste(float time)
    {
        justWallJumped = true;
        yield return new WaitForSeconds(time);
        justWallJumped = false;
    }

    public void ActivateInteractionText(bool readyToInteract)
    {
        interactText.gameObject.SetActive(readyToInteract);
    }

    public void ChangeSpriteDirection(bool direction)
    {
        if (direction && !facingRight)
        {
            facingRight = true;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        else if (!direction && facingRight)
        {
            facingRight = false;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void PlatformDrop()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)groundCheck.position + Vector2.down * 0.1f, new Vector2(0.8f, 0.2f), 0.0f, platformLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<Platform>(out var platform))
            {
                platform.RemoveCollision();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * wallCheckDistance);
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.left * wallCheckDistance);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            primaryAttackHeld = true;
            ResolveAttackOwner();
        }
        else if (context.canceled)
        {
            primaryAttackHeld = false;
            ResolveAttackOwner();
        }
    }
}
