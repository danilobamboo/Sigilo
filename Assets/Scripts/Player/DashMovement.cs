using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DashMovement : MonoBehaviour
{
    public KeyCode DashKey = KeyCode.LeftShift;
    public float DashSpeed = 20f;
    public float DashDuration = 0.15f;
    public float DashCooldown = 1f;

    public bool ResetVerticalVelocity = true;
    public bool UseGravityDuringDash = false;
    public int MaxAirDashes = 1;

    public Rigidbody PlayerBody;
    public RigidbodyMovement MovementScript;
    public Transform FeetTransform;
    public LayerMask FloorMask;

    private Vector3 dashDirection;
    private float dashTimeRemaining;
    private float cooldownTimeRemaining;
    private bool isDashing;
    private int airDashesUsed;
    private bool wasGroundedLastFrame;

    public bool IsDashing => isDashing;

    void Update()
    {
        bool grounded = Physics.CheckSphere(FeetTransform.position, 0.1f, FloorMask);
        if (grounded && !wasGroundedLastFrame)
        {
            airDashesUsed = 0;
        }
        wasGroundedLastFrame = grounded;

        if (cooldownTimeRemaining > 0f)
        {
            cooldownTimeRemaining -= Time.deltaTime;
        }

        bool canAirDash = grounded || MaxAirDashes < 0 || airDashesUsed < MaxAirDashes;

        if (Input.GetKeyDown(DashKey) && cooldownTimeRemaining <= 0f && !isDashing && canAirDash)
        {
            StartDash(grounded);
        }

        if (isDashing)
        {
            dashTimeRemaining -= Time.deltaTime;
            if (dashTimeRemaining <= 0f)
            {
                EndDash();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isDashing) return;

        Vector3 velocity = dashDirection * DashSpeed;
        if (!ResetVerticalVelocity)
        {
            velocity.y = PlayerBody.linearVelocity.y;
        }
        PlayerBody.linearVelocity = velocity;
    }

    private void StartDash(bool grounded)
    {
        Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));


        dashDirection = inputDir.sqrMagnitude > 0.01f
            ? transform.TransformDirection(inputDir).normalized
            : transform.forward;

        isDashing = true;
        dashTimeRemaining = DashDuration;
        cooldownTimeRemaining = DashCooldown;

        if (!grounded)
        {
            airDashesUsed++;
        }

        PlayerBody.useGravity = UseGravityDuringDash;


        if (MovementScript != null)
        {
            MovementScript.enabled = false;
        }
    }

    private void EndDash()
    {
        isDashing = false;
        PlayerBody.useGravity = true;

        if (MovementScript != null)
        {
            MovementScript.enabled = true;
        }
    }
}