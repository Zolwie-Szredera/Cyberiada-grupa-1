using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerHealth))]
public class Dash : MonoBehaviour
{
    enum DashState
    {
        Ready,
        Dashing,
        Cooldown
    }
    private DashState dashState = DashState.Ready;
    //this can be changed with: when playerSpeedX <= 12 (movespeed) end dash, but I can't bother for now
    public float dashDuration; //the calculations need to do a dash duration based on phycics are too complicated, so it remains static for now.
    public float dashCooldown;
    public float dashForce;
    public SpriteRenderer spriteRenderer;
    public GameObject dashAfterimagePrefab;
    private PlayerHealth playerHealth;
    private PlayerController playerController;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float dashTimer;
    private float dashDurationTimer;
    public void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }
    public void Update()
    {
        if (dashState == DashState.Dashing)
        {
            dashDurationTimer -= Time.deltaTime;

            // Create afterimage
            GameObject afterimage = Instantiate(dashAfterimagePrefab, transform.position, Quaternion.identity);
            DashAfterimage dashAfterimage = afterimage.GetComponent<DashAfterimage>();
            dashAfterimage.Setup(spriteRenderer.sprite, spriteRenderer.flipX, transform.localScale, spriteRenderer.color);

            //change to cooldown state
            if (dashDurationTimer <= 0f)
            {
                dashState = DashState.Cooldown;
                dashTimer = dashCooldown;
                playerHealth.isInvulnerable = false; //end invulnerability after dash
            }
        }
        else if (dashState == DashState.Cooldown)
        {
            dashTimer -= Time.deltaTime;
            //change to ready state
            if (dashTimer <= 0f)
            {
                dashState = DashState.Ready;
            }
        }
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PerformDash();
        }
    }
    //Dash will be unlocked via accessory(?) later on, but for now it is unlocked by default
    public void PerformDash()
    {
        moveInput = playerController.moveInput;
        if (moveInput.x != 0 && dashState == DashState.Ready)
        { //requirements ok, perform dash
            dashState = DashState.Dashing;
            playerHealth.isInvulnerable = true; //make player invulnerable during dash
            rb.linearVelocity += new Vector2(moveInput.x * dashForce, 0f);
            dashDurationTimer = dashDuration;
            Debug.Log($"Dash performed!");
        }
        else if (moveInput.x != 0)
        {
            Debug.Log("You can't dash right now, cooldown in progress");
        }
        else
        {
            Debug.Log("You can't dash without moving");
        }
    }
}
